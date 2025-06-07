using Confluent.Kafka;
using Model.Data;
using Model.Data.Repositories;
using Model.Domain;
using Model.Kafka;
using Model.Kafka.Messages;

namespace Worker.Hypotheses;

public class HypothesesMessageProcessor(
	ITopicInfoProvider topicInfoProvider,
	IKafkaMessageSerializer kafkaMessageSerializer,
	IKafkaProducer kafkaProducer,
	IDataExecutionContext dataExecutionContext)
	: IKafkaMessageProcessor<HypothesesMessage>
{
	public HypothesesMessage Deserialize(ConsumeResult<string, string> consumeResult)
		=> kafkaMessageSerializer.Deserialize<HypothesesMessage>(consumeResult.Message.Value);

	public async Task ProduceAsync(HypothesesMessage message, CancellationToken cancellationToken)
		=> await dataExecutionContext.ExecuteAsync(
			async repositories => await ProduceAsync(message, repositories, cancellationToken),
			cancellationToken);

	private async Task ProduceAsync(
		HypothesesMessage message,
		IRepositoryRegistry repositoryRegistry,
		CancellationToken cancellationToken)
	{
		var filters = message.NewsAnalyze.Hypotheses
			.Select(x => new UserProfileBatchFilter(x.Ticker, x.Probability))
			.ToHashSet();

		var userProfiles = await repositoryRegistry.UserProfileRepository.GetBatchAsync(filters, cancellationToken);

		var hypothesesByUsers = new List<HypothesisToUser>();
		foreach (var hypothesis in message.NewsAnalyze.Hypotheses)
		{
			foreach (var userProfile in userProfiles)
			{
				if (userProfile.Confidence >= hypothesis.Probability
				    && userProfile.Tickers.Any(t => t.Symbol == hypothesis.Ticker))
				{
					hypothesesByUsers.Add(new HypothesisToUser(userProfile.TelegramId, hypothesis));
				}
			}
		}

		var hypothesesByUser = hypothesesByUsers
			.GroupBy(x => x.TelegramId)
			.Select(x => new HypothesesToUser(x.Key, x.Select(y => y.Hypothesis).ToHashSet()))
			.ToList();

		var messages = hypothesesByUser
			.GroupBy(x => x.Hypotheses)
			.Select(x => new HypothesesForUsersMessage(
				x.Select(y => y.TelegramId).ToList(),
				message.NewsAnalyze with { Hypotheses = x.Key.ToList() }))
			.ToList();

		var topicInfo = topicInfoProvider.GetHypothesesForUsersTopicInfo();

		var tasks = messages.Select(msg =>
		{
			var key = Guid.NewGuid().ToString();
			var serializedMessage = kafkaMessageSerializer.Serialize(msg);
			return kafkaProducer.ProduceAsync(topicInfo, key, serializedMessage, cancellationToken);
		});

		await Task.WhenAll(tasks);
	}

	private record HypothesisToUser(long TelegramId, Hypothesis Hypothesis);

	private record HypothesesToUser(long TelegramId, HashSet<Hypothesis> Hypotheses);
}