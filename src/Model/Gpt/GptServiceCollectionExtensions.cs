using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Model.Gpt;

public static class GptServiceCollectionExtensions
{
	public static void AddGpt(this IServiceCollection services, IConfiguration configuration)
	{
		services
			.AddOptions<GptOptions>()
			.Bind(configuration.GetSection(GptOptions.SectionKey))
			.ValidateDataAnnotations()
			.ValidateOnStart();

		services.AddSingleton<IGptClient, OpenAiClient>();
	}
}