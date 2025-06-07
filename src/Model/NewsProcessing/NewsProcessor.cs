using Model.Gpt;
using Model.NewsProcessing.Results;

namespace Model.NewsProcessing;

public class NewsProcessor
{
    private GPTClient _gptClient = new("", "");
        
    public async Task<INewsProcessorResult> ProcessAsync(NewsProcessorRequest request)
    {
        var analyzeNewsResult = await _gptClient.AnalyzeNewsAsync(request.newsUrl);

        if (analyzeNewsResult is null)
            return new FailedNewsProcessorResult(["Sheet happens"]);

        return new SuccessNewsProcessorResult(analyzeNewsResult);
    }
}

public record RawNewsMessage
{
}