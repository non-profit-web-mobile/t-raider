using Microsoft.EntityFrameworkCore;
using Model.Domain;

namespace Model.Data.Repositories;

public class UserProfileRepository(DataContext context)
	: RepositoryBase<DataContext, UserProfile, int>(context), IUserProfileRepository
{
	public async Task<IReadOnlyList<UserProfile>> GetManyAsync(CancellationToken cancellationToken = default)
		=> await QuerySet
			.Include(x => x.Tickers)
			.ToListAsync(cancellationToken);
}