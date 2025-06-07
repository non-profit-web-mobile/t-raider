using Model.Domain;
using Model.Gpt;

namespace Model.NewsProcessing;

public class RawNewsProcessor(IGptClient gptClient) : IRawNewsProcessor
{
    public async Task ProcessAsync(RawNewsMessage rawNewsMessage)
    {
        var processResult = await gptClient.ProcessNewsAsync(rawNewsMessage.NewsLink);

        switch (processResult)
        {
            case NewsProcessorSuccessResult successResult:
                await Task.WhenAll(
                    PublishHypothesesAsync(successResult),
                    PublishRawNewsSuccessProcessingSignalAsync(successResult));
                break;
            case NewsProcessorErrorResult failedResult:
                await PublishRawNewsFailedProcessingSignalAsync(failedResult);
                break;
        }
    }

    private async Task PublishHypothesesAsync(INewsProcessorResult newsProcessingResult)
    {
    }

    private async Task PublishRawNewsSuccessProcessingSignalAsync(NewsProcessorSuccessResult result)
    {
    }

    private async Task PublishRawNewsFailedProcessingSignalAsync(NewsProcessorErrorResult result)
    {
    }
}