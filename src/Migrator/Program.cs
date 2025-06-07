using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Migrator;

var configuration = BuildConfiguration();
var serviceProvider = BuildServiceProvider(configuration);
Execute(serviceProvider);
return;


static IConfiguration BuildConfiguration()
{
	var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
	var contour = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

	var configurationBuilder = new ConfigurationBuilder();
	configurationBuilder.AddJsonFile(Path.Combine(baseDirectory, "appsettings.json"), optional: false);
	configurationBuilder.AddJsonFile(Path.Combine(baseDirectory, "appsettings.common.json"), optional: false);
	configurationBuilder.AddJsonFile(Path.Combine(baseDirectory, $"appsettings.{contour}.json"), optional: true);
	configurationBuilder.AddEnvironmentVariables();

	return configurationBuilder.Build();
}


static IServiceProvider BuildServiceProvider(IConfiguration configuration)
{
	var serviceCollection = new ServiceCollection();
	serviceCollection.AddMigrator(configuration);

	return serviceCollection.BuildServiceProvider();
}


static void Execute(IServiceProvider serviceProvider)
{
	var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
	logger.LogInformation($"{nameof(Migrator)} execution started.");

	try
	{
		var migrator = serviceProvider.GetRequiredService<IMigrationRunner>();
		migrator.MigrateUp();
	}
	catch (Exception ex)
	{
		logger.LogCritical(ex, $"{nameof(Migrator)} execution failed.");
		throw;
	}
}