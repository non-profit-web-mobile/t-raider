using Model.Kafka;
using Model.Kafka.Messages;

namespace Worker.Items;

public class ItemsService(
	ITopicInfoProvider topicInfoProvider,
	IKafkaMessageProcessor<ItemMessage> kafkaMessageProcessor,
	IKafkaConsumerFactory simpleKafkaConsumerFactory)
{
	public async Task ExecuteAsync(CancellationToken cancellationToken)
	{
		var topicInfo = topicInfoProvider.GetItemsTopicInfo();
		var groupId = $"{topicInfo.TopicName}.Consumer";

		var kafkaConsumer = simpleKafkaConsumerFactory.Create(topicInfo, groupId, kafkaMessageProcessor);
		await kafkaConsumer.ConsumeAsync(cancellationToken);
	}
}