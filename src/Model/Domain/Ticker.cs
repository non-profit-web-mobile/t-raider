namespace Model.Domain;

public class Ticker : EntityBase<int>
{
	public required string Symbol { get; set; }

	public ICollection<UserProfile> UserProfiles { get; set; } = new List<UserProfile>();
}