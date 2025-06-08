using Worker.Schedule;
using Worker.Tasks;

namespace Worker.RawNews;

public class RawNewsTask(
	IServiceProvider serviceProvider,
	ILogger<RawNewsTask> logger)
	: ScopedBackgroundTask(serviceProvider, TaskSchedule.Periodical(TimeSpan.FromMinutes(1)), logger)
{
	protected override async Task ExecuteIterationAsync(IServiceScope serviceScope, CancellationToken cancellationToken)
	{
		var service = serviceScope.ServiceProvider.GetRequiredService<RawNewsService>();
		await service.ExecuteAsync(cancellationToken);
	}
}