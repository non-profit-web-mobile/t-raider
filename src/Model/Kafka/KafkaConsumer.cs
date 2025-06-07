using Confluent.Kafka;

namespace Model.Kafka;

public class KafkaConsumer<TMessage>(
	TopicInfo topicInfo,
	string groupId,
	IKafkaMessageProcessor<TMessage> processor)
	: IKafkaConsumer<TMessage>
{
	public async Task ConsumeAsync(CancellationToken cancellationToken)
	{
		var consumerConfig = new ConsumerConfig
		{
			BootstrapServers = topicInfo.BootstrapServers,
			GroupId = groupId,
			AutoOffsetReset = AutoOffsetReset.Earliest,
			EnableAutoCommit = false
		};

		using var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
		consumer.Subscribe(topicInfo.TopicName);

		while (!cancellationToken.IsCancellationRequested)
		{
			var consumeResult = consumer.Consume(cancellationToken);
			var message =  processor.Deserialize(consumeResult);
			await processor.ProduceAsync(message, cancellationToken);
			consumer.Commit(consumeResult);
		}
	}
}