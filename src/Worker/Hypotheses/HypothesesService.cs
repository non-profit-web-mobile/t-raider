using Model.Kafka;
using Model.Kafka.Messages;

namespace Worker.Hypotheses;

public class HypothesesService(
	ITopicInfoProvider topicInfoProvider,
	IKafkaMessageProcessor<HypothesesMessage> kafkaMessageProcessor,
	IKafkaConsumerFactory kafkaConsumerFactory)
{
	public async Task ExecuteAsync(CancellationToken cancellationToken)
	{
		var topicInfo = topicInfoProvider.GetHypothesesTopicInfo();
		var groupId = $"{topicInfo.TopicName}.Consumer";

		var kafkaConsumer = kafkaConsumerFactory.Create(topicInfo, groupId, kafkaMessageProcessor);
		await kafkaConsumer.ConsumeAsync(cancellationToken);
	}
}