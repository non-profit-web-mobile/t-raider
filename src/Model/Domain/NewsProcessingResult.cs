namespace Model.Domain;

public interface INewsProcessorResult;

public record NewsProcessorErrorResult(string ErrorMessage): INewsProcessorResult;

public record NewsProcessorSuccessResult(
	string Brief,
	Uri SourceUrl,
	double Newsworthiness,
	string Explanation,
	IReadOnlyList<string> Tickers,
	IReadOnlyList<Hypothesis> Hypotheses
): INewsProcessorResult;

public record TickerInfo(
	string Symbol,
	decimal CurrentPrice
);

public enum ActionType
{
	Buy,
	Short,
	Hold,
	Other
}
