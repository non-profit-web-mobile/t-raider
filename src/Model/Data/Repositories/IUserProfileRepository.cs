using Model.Domain;

namespace Model.Data.Repositories;

public interface IUserProfileRepository : IRepository<UserProfile, int>
{
	Task<IReadOnlyList<UserProfile>> GetBatchAsync(
		IReadOnlySet<UserProfileBatchFilter> filters,
		CancellationToken cancellationToken = default);
}