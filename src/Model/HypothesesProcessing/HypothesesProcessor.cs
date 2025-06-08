using Model.Domain;
using Model.Kafka;
using Model.Kafka.MessageFactories;

namespace Model.HypothesesProcessing;

public class HypothesesProcessor(
	ITopicInfoProvider topicInfoProvider,
	IKafkaMessageSerializer kafkaMessageSerializer,
	IKafkaProducer kafkaProducer,
	IMessageToSendFactory kafkaMessageToSendFactory)
	: IHypothesesProcessor
{
	public async Task ProduceAsync(
		IReadOnlyCollection<UserProfile> userProfiles,
		NewsAnalyze newsAnalyze,
		CancellationToken cancellationToken)
	{
		var hypothesesForUsers = new List<HypothesisForUser>();
		foreach (var hypothesis in newsAnalyze.Hypotheses)
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
			.GroupBy(x => x.Hypotheses, new HypothesesHashSetEqualityComparer())
			.Select(x => kafkaMessageToSendFactory
				.Create(telegramIds: x.Select(y => y.TelegramId).ToList(),
					newsAnalyze: newsAnalyze with { Hypotheses = x.Key.ToList() }))
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

	private class HypothesesHashSetEqualityComparer : IEqualityComparer<HashSet<Hypothesis>>
	{
		public bool Equals(HashSet<Hypothesis>? x, HashSet<Hypothesis>? y)
		{
			if (x == null && y == null) return true;
			if (x == null || y == null) return false;
			return x.SetEquals(y);
		}

		public int GetHashCode(HashSet<Hypothesis> obj)
		{
			return obj.Aggregate(0, (current, hypothesis) => current ^ hypothesis.GetHashCode());
		}
	}
}