using Model.Domain;

namespace Model.NewsProcessing.Results;

public record SuccessNewsProcessorResult(NewsProcessingResult Result) : INewsProcessorResult;