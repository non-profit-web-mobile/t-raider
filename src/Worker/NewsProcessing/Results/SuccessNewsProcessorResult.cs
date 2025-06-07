using Model.Domain;

namespace Worker.NewsProcessing.Results;

public record SuccessNewsProcessorResult(IEnumerable<Hypothesis> Hypotheses) : INewsProcessorResult;