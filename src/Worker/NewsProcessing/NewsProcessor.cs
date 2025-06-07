namespace Worker.NewsProcessing;

public class NewsProcessor
{
    public async Task<INewsProcessorResult> ProcessAsync(NewsProcessorRequest request)
    {
        throw new NotImplementedException();
    }
}

public interface INewsProcessorResult
{
    
}

public class SuccessNewsProcessorResult
{
}

public class FailedNewsProcessorResult
{
}

public record NewsProcessorRequest
{
    public string Link { get; set; }
}