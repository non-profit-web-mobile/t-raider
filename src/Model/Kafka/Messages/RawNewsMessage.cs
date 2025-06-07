using System.Runtime.Serialization;

namespace Model.Kafka.Messages;

[DataContract]
public record RawNewsMessage(
	[property: DataMember(Name = "newsLink")] string NewsLink,
	[property: DataMember(Name = "sourceReliability")] int SourceReliability);