using Worker.Schedule;

namespace Worker.Tasks;

public abstract class ScopedBackgroundTask(
	IServiceProvider serviceProvider,
	TaskSchedule schedule,
	ILogger<BackgroundTask> logger)
	: BackgroundTask(schedule, logger)
{
	protected override async Task ExecuteIterationAsync(CancellationToken cancellationToken)
	{
		using var serviceScope = serviceProvider.CreateScope();
		await ExecuteIterationAsync(serviceScope, cancellationToken);
	}

	protected abstract Task ExecuteIterationAsync(IServiceScope serviceScope, CancellationToken cancellationToken);
}