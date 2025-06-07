using System.Runtime.Serialization;
using Model.Domain;

namespace Model.Kafka.Messages;

[DataContract]
public record HypothesesMessage(
	[property: DataMember(Name = "hypothesesContent")] HypothesesContentMessage HypothesesContent);

[DataContract]
public record HypothesesContentMessage(
	[property: DataMember(Name = "brief")] string Brief,
	[property: DataMember(Name = "sourceUrl")] Uri SourceUrl,
	[property: DataMember(Name = "newsworthiness")] double Newsworthiness,
	[property: DataMember(Name = "explanation")] string Explanation,
	[property: DataMember(Name = "tickers")] IReadOnlyList<string> Tickers,
	[property: DataMember(Name = "hypotheses")] IReadOnlyList<Hypothesis> Hypotheses);