namespace Model.Kafka;

public interface IKafkaProducer : IDisposable
{
	Task ProduceAsync(TopicInfo topicInfo, string key, string message, CancellationToken cancellationToken = default);
}