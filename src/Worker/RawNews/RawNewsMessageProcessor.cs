using Confluent.Kafka;
using Model.Kafka;
using Model.Kafka.Messages;
using Model.NewsProcessing;

namespace Worker.RawNews;

public class RawNewsMessageProcessor(
	IKafkaMessageSerializer kafkaMessageSerializer,
	IRawNewsProcessor rawNewsProcessor,
	ILogger<RawNewsMessageProcessor> logger)
	: IKafkaMessageProcessor<RawNewsMessage>
{
	public RawNewsMessage Deserialize(ConsumeResult<string, string> consumeResult)
		=> kafkaMessageSerializer.Deserialize<RawNewsMessage>(consumeResult.Message.Value);

	public async Task ProduceAsync(RawNewsMessage message, CancellationToken cancellationToken)
	{
		try
		{
			await rawNewsProcessor.ProcessAsync(message);
		}
		catch (Exception exception)
		{
			logger.LogError(exception, exception.Message);
		}
	}
}