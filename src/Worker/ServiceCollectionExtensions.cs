using Model.Data;
using Model.Gpt;
using Model.HypothesesProcessing;
using Model.Kafka;
using Model.Kafka.MessageFactories;
using Worker.Hypotheses;
using Worker.RawNews;

namespace Worker;

public static class ServiceCollectionExtensions
{
	public static void AddModel(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddKafka(configuration);
		services.AddKafkaMessages(configuration);
		services.AddData(configuration);
		services.AddGpt(configuration);
		services.AddRawNews();
		services.AddHypotheses();
		services.AddSingleton<IHypothesesProcessor, HypothesesProcessor>();
	}
}