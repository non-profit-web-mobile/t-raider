using Model.Data.Repositories;

namespace Model.Data;

public class RepositoryRegistry(DataContext dataContext) : IRepositoryRegistry
{
	public IUserProfileRepository UserProfileRepository { get; } = new UserProfileRepository(dataContext);
}