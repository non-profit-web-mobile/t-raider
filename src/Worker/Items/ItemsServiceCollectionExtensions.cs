using Model.Kafka;
using Model.Kafka.Messages;

namespace Worker.Items;

public static class ItemsServiceCollectionExtensions
{
	public static void AddItems(this IServiceCollection services)
	{
		services.AddHostedService<ItemsTask>();
		services.AddScoped<ItemsService>();
		services.AddScoped<IKafkaMessageProcessor<ItemMessage>, ItemsMessageProcessor>();
	}
}