using Confluent.Kafka;
using Model.Kafka;
using Model.Kafka.Messages;

namespace Worker.Hypotheses;

public class HypothesesMessageProcessor(
	IKafkaMessageSerializer kafkaMessageSerializer)
	: IKafkaMessageProcessor<HypothesesMessage>
{
	public HypothesesMessage Deserialize(ConsumeResult<string, string> consumeResult)
		=> kafkaMessageSerializer.Deserialize<HypothesesMessage>(consumeResult.Message.Value);

	public async Task ProduceAsync(HypothesesMessage message, CancellationToken cancellationToken)
	{
	}
}