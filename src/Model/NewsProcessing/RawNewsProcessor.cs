using Model.Domain;
using Model.Gpt;
using Model.Kafka;
using Model.Kafka.Messages;

namespace Model.NewsProcessing;

public class RawNewsProcessor(
    IGptClient gptClient,
    IKafkaProducer kafkaProducer,
    ITopicInfoProvider topicInfoProvider,
    IKafkaMessageSerializer kafkaMessageSerializer) : IRawNewsProcessor
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

    private async Task PublishHypothesesAsync(NewsProcessorSuccessResult result)
    {
        var topicInfo = topicInfoProvider.GetHypothesesTopicInfo();
        var key = Guid.NewGuid().ToString();
        var message = new HypothesesMessage(result.newsAnalyze);
        var serializedMessage = kafkaMessageSerializer.Serialize(message);
        
        await kafkaProducer.ProduceAsync(topicInfo, key, serializedMessage);
    }

    private async Task PublishRawNewsSuccessProcessingSignalAsync(NewsProcessorSuccessResult result)
    {
        // await kafkaProducer.ProduceAsync()
    }

    private async Task PublishRawNewsFailedProcessingSignalAsync(NewsProcessorErrorResult result)
    {
        // await kafkaProducer.ProduceAsync()
    }
}