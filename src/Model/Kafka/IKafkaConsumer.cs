namespace Model.Kafka;

public interface IKafkaConsumer<TMessage>
{
	Task ConsumeAsync(CancellationToken cancellationToken);
}