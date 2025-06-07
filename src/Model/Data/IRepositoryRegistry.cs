using Model.Data.Repositories;

namespace Model.Data;

public interface IRepositoryRegistry
{
	IItemsRepository Items { get; }
}