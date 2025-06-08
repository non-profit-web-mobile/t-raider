using Worker.Schedule;
using Worker.Tasks;

namespace Worker.NewsSummarySender;

public class NewsSummarySenderTask(
	IServiceProvider serviceProvider,
	ILogger<NewsSummarySenderTask> logger)
	: ScopedBackgroundTask(serviceProvider, TaskSchedule.Periodical(TimeSpan.FromMinutes(3)), logger)
{
	protected override async Task ExecuteIterationAsync(IServiceScope serviceScope, CancellationToken cancellationToken)
	{
		var service = serviceScope.ServiceProvider.GetRequiredService<INewsSummarySenderService>();
		await service.ExecuteAsync(cancellationToken);
	}
}