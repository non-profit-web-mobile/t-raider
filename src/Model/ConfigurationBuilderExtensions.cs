using Microsoft.Extensions.Configuration;

namespace Model;

public static class ConfigurationBuilderExtensions
{
	public static void AddAppSettings(this IConfigurationBuilder configurationBuilder)
	{
		var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
		var contour = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

		configurationBuilder.AddJsonFile(Path.Combine(baseDirectory, "appsettings.json"), optional: false);
		configurationBuilder.AddJsonFile(Path.Combine(baseDirectory, "appsettings.common.json"), optional: false);
		configurationBuilder.AddJsonFile(Path.Combine(baseDirectory, $"appsettings.{contour}.json"), optional: true);
		configurationBuilder.AddEnvironmentVariables();
	}
}