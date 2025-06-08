using Worker.Schedule;
using Worker.Tasks;

namespace Worker.NewsSummarySender;

public class NewsSummarySenderTask(
	IServiceProvider serviceProvider,
	ILogger<BackgroundTask> logger)
	: ScopedBackgroundTask(serviceProvider, TaskSchedule.Periodical(TimeSpan.FromHours(24)), logger)
{
	protected override async Task ExecuteIterationAsync(IServiceScope serviceScope, CancellationToken cancellationToken)
	{
		var service = serviceScope.ServiceProvider.GetRequiredService<NewsSummarySenderService>();
		await service.ExecuteAsync(cancellationToken);
	}
}