namespace Model.Domain;

public record NewsProcessorErrorResult(string ErrorMessage): INewsProcessorResult;