namespace Model.Domain;

public record NewsProcessorErrorResult(string NewsUrl, string ErrorMessage) : INewsProcessorResult;