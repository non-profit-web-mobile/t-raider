using Model.Data;
using Model.Kafka;
using Worker.RawNews;

namespace Worker;

public static class ServiceCollectionExtensions
{
	public static void AddModel(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddKafka(configuration);
		services.AddData(configuration);
		services.AddRawNews();
	}
}