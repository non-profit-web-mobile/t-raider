namespace Model.Kafka;

public record struct TopicInfo(string BootstrapServers, string TopicName);