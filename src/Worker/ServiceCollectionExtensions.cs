using Model.Data;
using Model.Gpt;
using Model.Kafka;
using Worker.Hypotheses;
using Worker.RawNews;

namespace Worker;

public static class ServiceCollectionExtensions
{
	public static void AddModel(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddKafka(configuration);
		services.AddData(configuration);
		services.AddGpt(configuration);
		services.AddRawNews();
		services.AddHypotheses();
	}
}