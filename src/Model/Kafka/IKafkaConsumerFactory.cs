namespace Model.Kafka;

public interface IKafkaConsumerFactory
{
	IKafkaConsumer<TMessage> Create<TMessage>(
		TopicInfo topicInfo,
		string groupId,
		IKafkaMessageProcessor<TMessage> processor);
}