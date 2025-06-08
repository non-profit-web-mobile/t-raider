namespace Services;

public class Startup(IConfiguration configuration)
{
	public void ConfigureServices(IServiceCollection services)
	{
		services.AddControllers();

		services.AddCors(options =>
			options.AddDefaultPolicy(policy =>
				policy
					.SetIsOriginAllowed(_ => true)
					.AllowCredentials()
					.AllowAnyHeader()
					.AllowAnyMethod()));

		services.AddResponseCompression(options =>
		{
			options.EnableForHttps = true;
		});

		services.AddModel(configuration);

		var seqSection = configuration.GetSection("Seq");
		if (seqSection.Exists())
			services.AddLogging(builder => builder.AddSeq(seqSection));
	}

	public void Configure(IApplicationBuilder app, IWebHostEnvironment environment)
	{
		app.UseRouting();

		app.UseCors();

		app.UseResponseCompression();

		app.UseEndpoints(builder =>
		{
			builder.MapControllers();
		});
	}
}