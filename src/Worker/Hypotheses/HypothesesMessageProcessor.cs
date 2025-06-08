using Confluent.Kafka;
using Model.Data;
using Model.HypothesesProcessing;
using Model.Kafka;
using Model.Kafka.Messages;

namespace Worker.Hypotheses;

public class HypothesesMessageProcessor(
	IKafkaMessageSerializer kafkaMessageSerializer,
	IDataExecutionContext dataExecutionContext,
	IHypothesesProcessor hypothesesProcessor)
	: IKafkaMessageProcessor<HypothesesMessage>
{
	public HypothesesMessage Deserialize(ConsumeResult<string, string> consumeResult)
		=> kafkaMessageSerializer.Deserialize<HypothesesMessage>(consumeResult.Message.Value);

	public async Task ProduceAsync(HypothesesMessage message, CancellationToken cancellationToken)
		=> await dataExecutionContext.ExecuteAsync(
			async repositories => await ProduceAsync(message, repositories, cancellationToken),
			cancellationToken);

	private async Task ProduceAsync(
		HypothesesMessage message,
		IRepositoryRegistry repositoryRegistry,
		CancellationToken cancellationToken)
	{
		var userProfiles = await repositoryRegistry.UserProfileRepository
			.GetManyAsync(cancellationToken);

		await hypothesesProcessor.ProduceAsync(userProfiles, message.NewsAnalyze, cancellationToken);
	}
}