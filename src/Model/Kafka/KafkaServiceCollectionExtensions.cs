using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Model.Kafka;

public static class KafkaServiceCollectionExtensions
{
	public static void AddKafka(this IServiceCollection services, IConfiguration configuration)
	{
		services
			.AddOptions<KafkaOptions>()
			.Bind(configuration.GetSection(KafkaOptions.SectionKey))
			.ValidateDataAnnotations()
			.ValidateOnStart();

		services.AddSingleton<ITopicInfoProvider, TopicInfoProvider>();
		services.AddSingleton<IKafkaProducer, KafkaProducer>();
		services.AddSingleton<IKafkaMessageSerializer, KafkaMessageSerializer>();

		services.AddSingleton<IKafkaConsumerFactory, KafkaConsumerFactory>();
	}
}