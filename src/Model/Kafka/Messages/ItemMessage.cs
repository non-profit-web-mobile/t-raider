using System.Runtime.Serialization;

namespace Model.Kafka.Messages;

[DataContract]
public record ItemMessage(
	[property: DataMember(Name = "value")] string Value);