using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Model.Domain;

[DataContract]
public record NewsAnalyze(
    [property: DataMember(Name = "brief")] string Brief,
    [property: DataMember(Name = "sourceUrl")] Uri SourceUrl,
    [property: DataMember(Name = "newsworthiness")] double Newsworthiness,
    [property: DataMember(Name = "explanation")] string Explanation,
    [property: DataMember(Name = "tickers")] IReadOnlyList<string> Tickers,
    [property: DataMember(Name = "hypotheses"), JsonPropertyName("hypotheses")] IReadOnlyList<Hypothesis> HypothesesList);
    