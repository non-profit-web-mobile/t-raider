using Microsoft.Extensions.DependencyInjection;

namespace Model.MessageClicks;

public static class MessageClicksServiceCollectionExtensions
{
    public static void AddMessageClicks(this IServiceCollection services)
    {
        services.AddSingleton<IMessageClickEncoder, MessageClickEncoder>();
    }
}