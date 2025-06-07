using System.Runtime.Serialization;
using Model.Domain;

namespace Model.Kafka.Messages;

[DataContract]
public record HypothesesMessage(
	[property: DataMember(Name = "newsAnalyze")] NewsAnalyze NewsAnalyze);