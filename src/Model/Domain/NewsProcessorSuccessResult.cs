namespace Model.Domain;

public record NewsProcessorSuccessResult(
    NewsAnalyze newsAnalyze
): INewsProcessorResult;