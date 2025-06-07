using System.Runtime.Serialization;

namespace Model.Domain;

[DataContract]
public record Hypothesis(
	[property: DataMember(Name = "ticker")] string Ticker,
	[property: DataMember(Name = "price")] decimal Price,
	[property: DataMember(Name = "action")] string Action,
	[property: DataMember(Name = "probability")] double Probability,
	[property: DataMember(Name = "explanation")] string Explanation,
	[property: DataMember(Name = "period")] int Period,
	[property: DataMember(Name = "tactics")] string Tactics,
	[property: DataMember(Name = "stopLoss")] decimal StopLoss,
	[property: DataMember(Name = "takeProfit")] decimal TakeProfit);