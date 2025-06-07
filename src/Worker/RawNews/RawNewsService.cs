using Model.Kafka;
using Model.Kafka.Messages;

namespace Worker.RawNews;

public class RawNewsService(
	ITopicInfoProvider topicInfoProvider,
	IKafkaMessageProcessor<RawNewsMessage> kafkaMessageProcessor,
	IKafkaConsumerFactory kafkaConsumerFactory)
{
	public async Task ExecuteAsync(CancellationToken cancellationToken)
	{
		var topicInfo = topicInfoProvider.GetRawNewsTopicInfo();
		var groupId = $"{topicInfo.TopicName}.Consumer";

		var kafkaConsumer = kafkaConsumerFactory.Create(topicInfo, groupId, kafkaMessageProcessor);
		await kafkaConsumer.ConsumeAsync(cancellationToken);
	}
}