using Microsoft.Extensions.Options;

namespace Model.Kafka;

public class TopicInfoProvider(IOptions<KafkaOptions> kafkaOptions) : ITopicInfoProvider
{
	public TopicInfo GetItemsTopicInfo() => new(kafkaOptions.Value.BootstrapServers, "Items");
}