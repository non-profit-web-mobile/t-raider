namespace Model.Kafka;

public class KafkaConsumerFactory : IKafkaConsumerFactory
{
	public IKafkaConsumer<TMessage> Create<TMessage>(
		TopicInfo topicInfo,
		string groupId,
		IKafkaMessageProcessor<TMessage> processor)
		=> new KafkaConsumer<TMessage>(topicInfo, groupId, processor);
}