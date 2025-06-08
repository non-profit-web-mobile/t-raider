using Model.Kafka;
using Model.Kafka.Messages;

namespace Worker.Hypotheses;

public static class HypothesesServiceCollectionExtensions
{
	public static void AddHypotheses(this IServiceCollection services)
	{
		services.AddHostedService<HypothesesTask>();
		services.AddScoped<HypothesesService>();
		services.AddScoped<IHypothesesMessageProcessor, HypothesesMessageProcessor>();
	}
}