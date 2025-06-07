using Confluent.Kafka;
using Model.Kafka;
using Model.Kafka.Messages;

namespace Worker.RawNews;

public class RawNewsMessageProcessor(
	IKafkaMessageSerializer kafkaMessageSerializer)
	: IKafkaMessageProcessor<RawNewsMessage>
{
	public RawNewsMessage Deserialize(ConsumeResult<string, string> consumeResult)
		=> kafkaMessageSerializer.Deserialize<RawNewsMessage>(consumeResult.Message.Value);

	public async Task ProduceAsync(RawNewsMessage message, CancellationToken cancellationToken)
	{
	}
}