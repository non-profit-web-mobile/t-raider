using Model.Domain;

namespace Model.Data.Repositories;

public class ItemsRepository(DataContext context) : RepositoryBase<DataContext, Item, int>(context), IItemsRepository;