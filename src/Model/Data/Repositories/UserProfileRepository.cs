using Microsoft.EntityFrameworkCore;
using Model.Domain;

namespace Model.Data.Repositories;

public class UserProfileRepository(DataContext context)
	: RepositoryBase<DataContext, UserProfile, int>(context), IUserProfileRepository
{
	public async Task<IReadOnlyList<UserProfile>> GetBatchAsync(
		IReadOnlySet<UserProfileBatchFilter> filters,
		CancellationToken cancellationToken = default)
		=> await QuerySet
			.Where(userProfile =>
				filters
					.Any(filter =>
						userProfile.Tickers.Any(ticker => ticker.Symbol == filter.Ticker)
						&& userProfile.Confidence >= filter.Probability))
			.ToListAsync(cancellationToken);
}