using Model.Domain;
using Model.NewsProcessing.Results;

namespace Model.NewsProcessing;

public class RawNewsConsumer
{
    private readonly NewsProcessor _newsProcessor = new NewsProcessor();
    
    public async Task Process(RawNewsMessage rawNews)
    {
        var newsProcessorRequest = new NewsProcessorRequest(newsUrl: "https://www.rbc.ru/finances/06/06/2025/68403e4c9a7947dda98935cf");
        var processResult = await _newsProcessor.ProcessAsync(newsProcessorRequest);
        
        switch (processResult)
        {
            case SuccessNewsProcessorResult successResult:
                await Task.WhenAll(
                    PublishHypothesesAsync(successResult.Result),
                    PublishRawNewsSuccessProcessingSignalAsync(newsProcessorRequest, successResult.Result));
                break;
            case FailedNewsProcessorResult failedResult:
                await PublishRawNewsFailedProcessingSignalAsync(newsProcessorRequest, failedResult.Fails);
                break;
        }
    }

    private async Task PublishHypothesesAsync(NewsProcessingResult newsProcessingResult)
    {
    }

    private async Task PublishRawNewsSuccessProcessingSignalAsync(NewsProcessorRequest request, NewsProcessingResult newsProcessingResult)
    {
    }

    private async Task PublishRawNewsFailedProcessingSignalAsync(NewsProcessorRequest request, IEnumerable<string> fails)
    {
    }
}