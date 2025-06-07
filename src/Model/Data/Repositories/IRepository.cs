using Model.Domain;

namespace Model.Data.Repositories;

public interface IRepository<TEntity, in TId>
	where TEntity : EntityBase<TId>
	where TId : IEquatable<TId>
{
	Task CreateAsync(TEntity entity, CancellationToken cancellationToken = default);
    
	Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

	Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

	Task<TEntity> GetByIdAsync(TId id, CancellationToken cancellationToken = default);

	Task<TEntity?> TryGetByIdAsync(TId id, CancellationToken cancellationToken = default);
}