using Model.Data;
using Model.Kafka;
using Model.Kafka.MessageFactories;
using Model.MessageClicks;

namespace Services;

public static class ServiceCollectionExtensions
{
	public static void AddModel(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddKafka(configuration);
		services.AddData(configuration);
		services.AddKafkaMessages(configuration);
		services.AddMessageClicks();
	}
}