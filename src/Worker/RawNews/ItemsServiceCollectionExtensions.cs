using Model.Kafka;
using Model.Kafka.Messages;
using Model.NewsProcessing;

namespace Worker.RawNews;

public static class RawNewsServiceCollectionExtensions
{
	public static void AddRawNews(this IServiceCollection services)
	{
		services.AddHostedService<RawNewsTask>();
		services.AddScoped<RawNewsService>();
		services.AddScoped<IKafkaMessageProcessor<RawNewsMessage>, RawNewsMessageProcessor>();
		services.AddScoped<IRawNewsProcessor, RawNewsProcessor>();
	}
}