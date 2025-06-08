using Confluent.Kafka;
using Model.Domain;
using Model.Kafka;
using Model.Kafka.Messages;

namespace Worker.NewsSummarySender;

public interface INewsSummarySenderProcessor : IKafkaMessageProcessor<HypothesesMessage>;
public class NewsSummarySenderProcessor(NewsSummarySenderService service) : INewsSummarySenderProcessor
{
	public HypothesesMessage Deserialize(ConsumeResult<string, string> consumeResult)
	{
		return service.KafkaMessageSerializer.Deserialize<HypothesesMessage>(consumeResult.Message.Value);
	}

	public async Task ProduceAsync(HypothesesMessage message, CancellationToken cancellationToken)
	{
		await service.DataExecutionContext.ExecuteAsync(async repositories =>
		{
			// 1. Получаем топ-5 гипотез по вероятности
			var topHypotheses = message.NewsAnalyze.Hypotheses
				.OrderByDescending(h => h.Probability)
				.Take(5)
				.ToList();

			if (!topHypotheses.Any())
				return;

			// 2. Получаем всех пользователей с SummaryEnabled = true
			var userProfiles = await repositories.UserProfileRepository.GetManyAsync(cancellationToken);
			var summaryUsers = userProfiles.Where(u => u.SummaryEnabled).ToList();

			// 3. Матчим пользователей с гипотезами по тикерам
			var userHypotheses = new List<(long TelegramId, List<Hypothesis> Hypotheses)>();
			foreach (var user in summaryUsers)
			{
				var matched = topHypotheses
					.Where(h => user.Tickers.Any(t => t.Symbol == h.Ticker))
					.ToList();
				if (matched.Any())
					userHypotheses.Add((user.TelegramId, matched));
			}

			// 4. Для каждого пользователя формируем и отправляем сообщение
			foreach (var (telegramId, hypotheses) in userHypotheses)
			{
				var newsAnalyze = message.NewsAnalyze with { Hypotheses = hypotheses };
				var msg = service.MessageToSendFactory.Create(new List<long> { telegramId }, newsAnalyze);
				var serialized = service.KafkaMessageSerializer.Serialize(msg);
				var topic = service.TopicInfoProvider.GetHypothesesForUsersTopicInfo();
				var key = Guid.NewGuid().ToString();
				await service.KafkaProducer.ProduceAsync(topic, key, serialized, cancellationToken);
			}
		}, cancellationToken);
	}
}