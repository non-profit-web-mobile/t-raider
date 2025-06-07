namespace Worker.Schedule;

public abstract class TaskSchedule
{
	public static TaskSchedule Periodical(TimeSpan runInterval) => new PeriodicalTaskSchedule(runInterval);

	public abstract Task WaitUntilNextRunAsync(CancellationToken cancellationToken);
}