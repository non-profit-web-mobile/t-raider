using Model.Domain;
using Model.Gpt;
using Model.Kafka;
using Model.Kafka.MessageFactories;
using Model.Kafka.Messages;

namespace Model.NewsProcessing;

public class RawNewsProcessor(
    IGptClient gptClient,
    IKafkaProducer kafkaProducer,
    ITopicInfoProvider topicInfoProvider,
    IHypothesesMessageMessageFactory hypothesesMessageMessageFactory,
    IAdminSignalMessageFactory adminSignalMessageFactory,
    IKafkaMessageSerializer kafkaMessageSerializer) : IRawNewsProcessor
{
    public async Task ProcessAsync(RawNewsMessage rawNewsMessage)
    {
        var processResult = await gptClient.ProcessNewsAsync(rawNewsMessage.NewsLink, rawNewsMessage.SourceReliability);

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
        var message = hypothesesMessageMessageFactory.Create(result);

        var serializedMessage = kafkaMessageSerializer.Serialize(message);

        await kafkaProducer.ProduceAsync(topicInfo, key, serializedMessage);
    }

    private async Task PublishRawNewsSuccessProcessingSignalAsync(NewsProcessorSuccessResult result)
    {
        var topicInfo = topicInfoProvider.GetAdminSignalsTopicInfo();

        var key = Guid.NewGuid().ToString();
        var message = adminSignalMessageFactory.Create(result);

        var serializedMessage = kafkaMessageSerializer.Serialize(message);

        await kafkaProducer.ProduceAsync(topicInfo, key, serializedMessage);
    }

    private async Task PublishRawNewsFailedProcessingSignalAsync(NewsProcessorErrorResult result)
    {
        var topicInfo = topicInfoProvider.GetAdminSignalsTopicInfo();

        var key = Guid.NewGuid().ToString();
        var message = adminSignalMessageFactory.Create(result);

        var serializedMessage = kafkaMessageSerializer.Serialize(message);

        await kafkaProducer.ProduceAsync(topicInfo, key, serializedMessage);
    }
}