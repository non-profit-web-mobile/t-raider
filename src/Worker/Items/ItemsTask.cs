using Worker.Schedule;
using Worker.Tasks;

namespace Worker.Items;

public class ItemsTask(
	IServiceProvider serviceProvider,
	ILogger<BackgroundTask> logger)
	: ScopedBackgroundTask(serviceProvider, TaskSchedule.Periodical(TimeSpan.FromMinutes(1)), logger)
{
	protected override async Task ExecuteIterationAsync(IServiceScope serviceScope, CancellationToken cancellationToken)
	{
		var service = serviceScope.ServiceProvider.GetRequiredService<ItemsService>();
		await service.ExecuteAsync(cancellationToken);
	}
}