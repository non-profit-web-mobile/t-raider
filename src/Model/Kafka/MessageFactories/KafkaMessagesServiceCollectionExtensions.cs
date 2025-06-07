using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Model.Kafka.MessageFactories;

public static class KafkaMessagesServiceCollectionExtensions
{
    public static void AddKafkaMessages(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IHypothesesMessageMessageFactory, HypothesesMessageMessageFactory>();
        services.AddSingleton<IAdminSignalMessageFactory, AdminSignalMessageFactory>();
    }
}