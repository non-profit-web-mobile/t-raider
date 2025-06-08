using Microsoft.Extensions.DependencyInjection;

namespace Model.HypothesesProcessing;

public static class HypothesesProcessingServiceCollectionExtensions
{
    public static void AddHypothesesProcessing(this IServiceCollection services)
    {
        services.AddSingleton<IHypothesesProcessor, HypothesesProcessor>();
    }
}