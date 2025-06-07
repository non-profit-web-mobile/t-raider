using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Model.Data;

public static class DataServiceCollectionExtensions
{
	public static void AddData(this IServiceCollection services, IConfiguration configuration)
	{
		var connectionString = configuration.GetConnectionString("TRaiderDatabase")
			?? throw new InvalidOperationException("Connection string is not configured.");

		services.AddDbContext<DataContext>(
			builder => builder.UseNpgsql(
				connectionString,
				options => options.EnableRetryOnFailure(
					3,
					TimeSpan.FromSeconds(10),
					null)));

		services.AddScoped<IRepositoryRegistry, RepositoryRegistry>();
		services.AddScoped<IDataExecutionContext, DataExecutionContext>();
	}
}