using System.Runtime.Serialization;

namespace Model.Kafka.Messages;

[DataContract]
public record AdminSignalMessage(
    [property: DataMember(Name = "message")] string Message);