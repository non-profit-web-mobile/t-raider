using Microsoft.Extensions.Options;

namespace Model.Kafka;

public class TopicInfoProvider(IOptions<KafkaOptions> kafkaOptions) : ITopicInfoProvider
{
	public TopicInfo GetRawNewsTopicInfo() => new(kafkaOptions.Value.BootstrapServers, "RawNews");
}