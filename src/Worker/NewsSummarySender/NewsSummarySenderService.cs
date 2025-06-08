using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Model.Data;
using Model.Kafka;
using Model.Kafka.MessageFactories;
using Model.Kafka.Messages;
using Model.Domain;

namespace Worker.NewsSummarySender;

public class NewsSummarySenderService(
	ITopicInfoProvider topicInfoProvider,
	IKafkaMessageSerializer kafkaMessageSerializer,
	IDataExecutionContext dataExecutionContext,
	IMessageToSendFactory messageToSendFactory,
	IKafkaProducer kafkaProducer)
{
	public ITopicInfoProvider TopicInfoProvider { get; } = topicInfoProvider;
	public IKafkaMessageSerializer KafkaMessageSerializer { get; } = kafkaMessageSerializer;
	public IDataExecutionContext DataExecutionContext { get; } = dataExecutionContext;
	public IMessageToSendFactory MessageToSendFactory { get; } = messageToSendFactory;
	public IKafkaProducer KafkaProducer { get; } = kafkaProducer;

	public async Task ExecuteAsync(CancellationToken cancellationToken)
	{
		var topicInfo = TopicInfoProvider.GetHypothesesTopicInfo();
		var consumerConfig = new ConsumerConfig
		{
			BootstrapServers = topicInfo.BootstrapServers,
			GroupId = $"{topicInfo.TopicName}.NewsSummarySender.Batch",
			AutoOffsetReset = AutoOffsetReset.Earliest,
			EnableAutoCommit = false
		};

		var allNews = new List<NewsAnalyze>();
		using (var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build())
		{
			consumer.Subscribe(topicInfo.TopicName);
			int emptyPolls = 0;
			const int maxEmptyPolls = 5;
			while (!cancellationToken.IsCancellationRequested && emptyPolls < maxEmptyPolls)
			{
				var consumeResult = consumer.Consume(TimeSpan.FromSeconds(1));
				if (consumeResult == null)
				{
					emptyPolls++;
					continue;
				}

				emptyPolls = 0;
				var message = KafkaMessageSerializer.Deserialize<HypothesesMessage>(consumeResult.Message.Value);
				allNews.Add(message.NewsAnalyze);
			}
		}

		if (!allNews.Any())
			return;

		await DataExecutionContext.ExecuteAsync(async repositories =>
		{
			var userProfiles = await repositories.UserProfileRepository.GetManyAsync(cancellationToken);
			var summaryUsers = userProfiles.Where(u => u.SummaryEnabled).ToList();

			var qualifedNews = allNews.OrderByDescending(x => x.Newsworthiness).Take(5);
			
			foreach (var user in summaryUsers)
			{
				var relevantNews = new List<NewsAnalyze>();
				foreach (var news in qualifedNews)
				{
					var relevantHypotheses = news.Hypotheses
						.Where(h => user.Tickers.Any(t => t.Symbol == h.Ticker)
						            && user.Confidence < h.Probability && h.Probability >= 0.8)
						.OrderByDescending(x => x.Probability)
						.Take(1)
						.ToList();
					if (relevantHypotheses.Any())
					{
						relevantNews.Add(news with { Hypotheses = relevantHypotheses });
					}
				}

				if (relevantNews.Any())
				{
					var msg = MessageToSendFactory.Create(new List<long> { user.TelegramId }, relevantNews);
					var serialized = KafkaMessageSerializer.Serialize(msg);
					var topic = TopicInfoProvider.GetHypothesesForUsersTopicInfo();
					var key = Guid.NewGuid().ToString();
					await KafkaProducer.ProduceAsync(topic, key, serialized, cancellationToken);
				}
			}
		}, cancellationToken);
	}
}