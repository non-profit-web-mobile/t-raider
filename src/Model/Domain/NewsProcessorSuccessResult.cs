namespace Model.Domain;

public record NewsProcessorSuccessResult(
    string NewsUrl,
    NewsAnalyze NewsAnalyze,
    int UsageTotalTokenCount,
    TimeSpan ProcessingDurationTime
) : INewsProcessorResult;