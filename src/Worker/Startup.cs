namespace Worker;

public class Startup(IConfiguration configuration)
{
	public void ConfigureServices(IServiceCollection services)
	{
		services.AddControllers();

		services.AddModel(configuration);
	}

	public void Configure(IApplicationBuilder app, IWebHostEnvironment environment)
	{
		app.UseRouting();

		app.UseEndpoints(builder =>
		{
			builder.MapControllers();
		});
	}
}