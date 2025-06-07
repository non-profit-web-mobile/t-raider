namespace Model.Domain;

public class Item : EntityBase<int>
{
	public string Value { get; set; } = default!;
}