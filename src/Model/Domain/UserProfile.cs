namespace Model.Domain;

public class UserProfile : EntityBase<int>
{
	public required long TelegramId { get; set; }

	public required bool StreamEnabled { get; set; }

	public required bool SummaryEnabled { get; set; }

	public required ICollection<Ticker> Tickers { get; set; }
}