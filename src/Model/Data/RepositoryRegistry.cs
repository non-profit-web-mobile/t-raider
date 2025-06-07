using Model.Data.Repositories;

namespace Model.Data;

public class RepositoryRegistry(DataContext dataContext) : IRepositoryRegistry
{
	public IItemsRepository Items { get; } = new ItemsRepository(dataContext);
}