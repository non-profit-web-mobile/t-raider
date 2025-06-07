namespace Worker.NewsProcessing.Results;

public record FailedNewsProcessorResult(IEnumerable<string> Fails) : INewsProcessorResult;