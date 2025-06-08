namespace Services;

public class Startup(IConfiguration configuration)
{
	public void ConfigureServices(IServiceCollection services)
	{
		services.AddControllers();

		services.AddModel(configuration);

		var seqSection = configuration.GetSection("Seq");
		if (seqSection.Exists())
			services.AddLogging(builder => builder.AddSeq(seqSection));
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