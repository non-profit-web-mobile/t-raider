namespace Model.Domain;

public record NewsProcessorSuccessResult(
    string Brief,
    Uri SourceUrl,
    double Newsworthiness,
    string Explanation,
    IReadOnlyList<string> Tickers,
    IReadOnlyList<Hypothesis> Hypotheses
): INewsProcessorResult;