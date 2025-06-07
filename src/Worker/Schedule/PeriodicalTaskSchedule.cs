namespace Worker.Schedule;

public class PeriodicalTaskSchedule(TimeSpan runInterval) : TaskSchedule
{
	public override Task WaitUntilNextRunAsync(CancellationToken cancellationToken)
		=> Task.Delay(runInterval, cancellationToken);
}