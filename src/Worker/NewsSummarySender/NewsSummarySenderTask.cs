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
		// читаем весь топик гипотез, берем только топовые, 5 штук

		// берем все юзеров, которые хотят получать сводку новостей summaryenabled = true

		// матчим юзеров с гипотезами

		// отправляем сообщение в телеграм
		
	}
}