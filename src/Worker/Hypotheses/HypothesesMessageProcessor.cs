using Confluent.Kafka;
using Model.Data;
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
		var userProfiles = await repositoryRegistry.UserProfileRepository
			.GetManyAsync(cancellationToken);

		var hypothesesForUsers = new List<HypothesisForUser>();
		foreach (var hypothesis in message.NewsAnalyze.Hypotheses)
		{
			foreach (var userProfile in userProfiles)
			{
				if (userProfile.Confidence < hypothesis.Probability
					&& userProfile.Tickers.Any(t => t.Symbol == hypothesis.Ticker))
				{
					hypothesesForUsers.Add(new HypothesisForUser(userProfile.TelegramId, hypothesis));
				}
			}
		}

		var hypothesesForUser = hypothesesForUsers
			.GroupBy(x => x.TelegramId)
			.Select(x => new HypothesesForUser(x.Key, x.Select(y => y.Hypothesis).ToHashSet()))
			.ToList();

		var messages = hypothesesForUser
			.GroupBy(x => x.Hypotheses)
			.Select(x => new HypothesesForUsersMessage(
				x.Select(y => y.TelegramId).ToList(),
				message.NewsAnalyze with { Hypotheses = x.Key.ToList() }))
			.ToList();

		var topicInfo = topicInfoProvider.GetHypothesesForUsersTopicInfo();

		var tasks = messages.Select(x =>
		{
			var key = Guid.NewGuid().ToString();
			var serializedMessage = kafkaMessageSerializer.Serialize(x);
			return kafkaProducer.ProduceAsync(topicInfo, key, serializedMessage, cancellationToken);
		});

		await Task.WhenAll(tasks);
	}

	private record HypothesisForUser(long TelegramId, Hypothesis Hypothesis);

	private record HypothesesForUser(long TelegramId, HashSet<Hypothesis> Hypotheses);
}