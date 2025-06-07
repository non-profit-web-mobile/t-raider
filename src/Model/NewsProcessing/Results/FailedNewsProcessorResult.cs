namespace Model.NewsProcessing.Results;

public record FailedNewsProcessorResult(IEnumerable<string> Fails) : INewsProcessorResult;