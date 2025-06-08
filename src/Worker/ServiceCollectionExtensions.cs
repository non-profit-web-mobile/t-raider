using Microsoft.Extensions.DependencyInjection;
using Model.Data;
using Model.Gpt;
using Model.HypothesesProcessing;
using Model.Kafka;
using Model.Kafka.MessageFactories;
using Model.MessageClicks;
using Worker.Hypotheses;
using Worker.NewsSummarySender;
using Worker.RawNews;
using Worker.Tasks;

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
		services.AddHypothesesProcessing();
		services.AddMessageClicks();
	}
}