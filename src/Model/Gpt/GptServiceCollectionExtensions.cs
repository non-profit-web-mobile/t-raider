using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Model.Gpt;

public static class GptServiceCollectionExtensions
{
	public static void AddGpt(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddSingleton<IGptClient, OpenAIClient>();
	}
}