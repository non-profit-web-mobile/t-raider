namespace Model.Domain;

public record NewsProcessorSuccessResult(
    string newsUrl,
    NewsAnalyze newsAnalyze
): INewsProcessorResult;