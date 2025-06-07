namespace Model.Domain;

public abstract class EntityBase<TId> where TId : IEquatable<TId>
{
	public TId Id { get; protected init; } = default!;
}