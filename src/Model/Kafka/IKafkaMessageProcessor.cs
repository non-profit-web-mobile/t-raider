using Confluent.Kafka;

namespace Model.Kafka;

public interface IKafkaMessageProcessor<TMessage>
{
	TMessage Deserialize(ConsumeResult<string, string> consumeResult);

	Task ProduceAsync(TMessage message, CancellationToken cancellationToken);
}