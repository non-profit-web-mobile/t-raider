using System.Reflection;
using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Migrator;

public static class ServiceCollectionExtensions
{
	public static void AddMigrator(this IServiceCollection services, IConfiguration configuration)
	{
		var connectionString = configuration.GetConnectionString("TRaiderDatabase")
			?? throw new InvalidOperationException("Connection string is not configured.");
		var executingAssembly = Assembly.GetExecutingAssembly();

		services.AddLogging(loggingBuilder => loggingBuilder.AddFluentMigratorConsole());
		services.AddFluentMigratorCore();
		services.ConfigureRunner(runnerBuilder => runnerBuilder
			.AddPostgres()
			.WithGlobalConnectionString(connectionString)
			.ScanIn(executingAssembly));
	}
}