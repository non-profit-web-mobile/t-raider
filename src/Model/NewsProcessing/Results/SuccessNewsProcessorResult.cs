using Model.Domain;

namespace Model.NewsProcessing.Results;

public record SuccessNewsProcessorResult(IEnumerable<Hypothesis> Hypotheses) : INewsProcessorResult;