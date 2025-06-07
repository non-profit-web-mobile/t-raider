using Worker.Schedule;

namespace Worker.Tasks;

public class ItemsTask(
	ILogger<BackgroundTask> logger)
	: BackgroundTask(TaskSchedule.Periodical(TimeSpan.FromMinutes(1)), logger)
{
	protected override Task ExecuteIterationAsync(CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}
}