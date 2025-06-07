using System.Runtime.Serialization;

namespace Model.Domain;

[DataContract]
public record TickerInfo(
	[property: DataMember(Name = "symbol")] string Symbol,
	[property: DataMember(Name = "currentPrice")] decimal CurrentPrice);