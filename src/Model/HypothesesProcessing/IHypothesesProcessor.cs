using Model.Domain;

namespace Model.HypothesesProcessing;

public interface IHypothesesProcessor
{
	Task ProduceAsync(
		IReadOnlyCollection<UserProfile> userProfiles,
		NewsAnalyze newsAnalyze,
		CancellationToken cancellationToken);
}