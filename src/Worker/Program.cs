using Model;

namespace Worker;

public static class Program
{
	public static async Task Main(string[] args) => await CreateHostBuilder(args).Build().RunAsync();

	private static IHostBuilder CreateHostBuilder(string[] args)
		=> Host
			.CreateDefaultBuilder(args)
			.ConfigureAppConfiguration((_, configurationBuilder) => configurationBuilder.AddAppSettings())
			.ConfigureWebHostDefaults(webHostBuilder => webHostBuilder.UseStartup<Startup>())
			.UseDefaultServiceProvider((_, options) =>
			{
				options.ValidateScopes = true;
				options.ValidateOnBuild = true;
			});
}