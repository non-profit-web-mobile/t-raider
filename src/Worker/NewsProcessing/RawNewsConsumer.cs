using Model.Domain;
using Worker.NewsProcessing.Results;

namespace Worker.NewsProcessing;

public class RawNewsConsumer
{
    private readonly NewsProcessor _newsProcessor = new NewsProcessor();
    
    public async Task Process(RawNewsMessage rawNews)
    {
        var newsProcessorRequest = new NewsProcessorRequest(Link: "https://www.rbc.ru/finances/06/06/2025/68403e4c9a7947dda98935cf");
        var processResult = await _newsProcessor.ProcessAsync(newsProcessorRequest);
        
        switch (processResult)
        {
            case SuccessNewsProcessorResult successResult:
                await Task.WhenAll(
                    PublishHypothesesAsync(successResult.Hypotheses),
                    PublishRawNewsSuccessProcessingSignalAsync(newsProcessorRequest, successResult.Hypotheses));
                break;
            case FailedNewsProcessorResult failedResult:
                await PublishRawNewsFailedProcessingSignalAsync(newsProcessorRequest, failedResult.Fails);
                break;
        }
    }

    private async Task PublishHypothesesAsync(IEnumerable<Hypothesis> hypothesis)
    {
    }

    private async Task PublishRawNewsSuccessProcessingSignalAsync(NewsProcessorRequest request, IEnumerable<Hypothesis> hypotheses)
    {
    }

    private async Task PublishRawNewsFailedProcessingSignalAsync(NewsProcessorRequest request, IEnumerable<string> fails)
    {
    }
}