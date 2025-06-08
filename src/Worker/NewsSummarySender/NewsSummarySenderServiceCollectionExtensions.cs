using Model.Kafka;
using Model.Kafka.Messages;

namespace Worker.NewsSummarySender;

public static class NewsSummarySenderServiceCollectionExtensions
{
	public static void AddNewsSummarySender(this IServiceCollection services)
	{
		services.AddHostedService<NewsSummarySenderTask>();
		services.AddScoped<NewsSummarySenderService>();
		services.AddScoped<INewsSummarySenderProcessor, NewsSummarySenderProcessor>();
	}
}