using Worker.Schedule;

namespace Worker.Tasks;

public abstract class BackgroundTask(
	TaskSchedule schedule,
	ILogger<BackgroundTask> logger)
	: BackgroundService
{
	protected override async Task ExecuteAsync(CancellationToken cancellationToken)
	{
		while (!cancellationToken.IsCancellationRequested)
		{
			try
			{
				await WaitCompletionOrCancellationAsync(ExecuteIterationAsync, cancellationToken);
			}
			catch (Exception exception)
			{
				logger.LogError(exception, "Failed to execute task iteration.");
			}

			if (cancellationToken.IsCancellationRequested)
				return;

			try
			{
				await WaitCompletionOrCancellationAsync(schedule.WaitUntilNextRunAsync, cancellationToken);
			}
			catch (Exception exception)
			{
				logger.LogCritical(exception, "Task schedule failed with exception. Will stop task now.");
				throw;
			}
		}
	}

	protected abstract Task ExecuteIterationAsync(CancellationToken cancellationToken);

	private static async Task WaitCompletionOrCancellationAsync(
		Func<CancellationToken, Task> action,
		CancellationToken cancellationToken)
	{
		try
		{
			await action(cancellationToken);
		}
		catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
		{
		}
		catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
		{
		}
	}
}