using System.Runtime.Serialization;

namespace Model.Domain;

[DataContract]
public record Hypothesis(
	[property: DataMember(Name = "tickers")] IReadOnlyList<TickerInfo> Tickers,
	[property: DataMember(Name = "action")] ActionType Action,
	[property: DataMember(Name = "probability")] double Probability,
	[property: DataMember(Name = "period")] int Period,
	[property: DataMember(Name = "tactics")] string Tactics,
	[property: DataMember(Name = "entryEvent")] string EntryEvent,
	[property: DataMember(Name = "stopLoss")] decimal StopLoss,
	[property: DataMember(Name = "takeProfit")] decimal TakeProfit);