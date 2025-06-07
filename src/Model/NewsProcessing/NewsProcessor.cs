using Model.NewsProcessing.Results;

namespace Model.NewsProcessing;

public class NewsProcessor
{
    public async Task<INewsProcessorResult> ProcessAsync(NewsProcessorRequest request)
    {
        throw new NotImplementedException();
    }
}

public record NewsProcessorRequest(string Link);

public record RawNewsMessage
{
}