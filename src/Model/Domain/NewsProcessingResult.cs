namespace Model.Domain;

public record NewsProcessingResult(SuccessResult? SuccessResult, ErrorResult? ErrorResult);

public record ErrorResult(string ErrorMessage);

public record SuccessResult(
	string Brief,
	Uri SourceUrl,
	double Newsworthiness,
	string Explanation,
	IReadOnlyList<string> Tickers,
	IReadOnlyList<Hypothesis> Hypotheses
);

public record Hypothesis(
	IReadOnlyList<TickerInfo> Tickers,
	ActionType Action,
	double Probability,
	int Period,
	string Tactics,
	string EntryEvent,
	decimal StopLoss,
	decimal TakeProfit
);

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
