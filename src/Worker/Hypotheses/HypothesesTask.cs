using Worker.Schedule;
using Worker.Tasks;

namespace Worker.Hypotheses;

public class HypothesesTask(
	IServiceProvider serviceProvider,
	ILogger<BackgroundTask> logger)
	: ScopedBackgroundTask(serviceProvider, TaskSchedule.Periodical(TimeSpan.FromMinutes(1)), logger)
{
	protected override async Task ExecuteIterationAsync(IServiceScope serviceScope, CancellationToken cancellationToken)
	{
		var service = serviceScope.ServiceProvider.GetRequiredService<HypothesesService>();
		await service.ExecuteAsync(cancellationToken);
	}
}