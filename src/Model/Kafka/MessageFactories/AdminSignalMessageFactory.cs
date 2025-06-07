using Model.Domain;
using Model.Kafka.Messages;

namespace Model.Kafka.MessageFactories;

public class AdminSignalMessageFactory : IAdminSignalMessageFactory
{
    public AdminSignalMessage Create(NewsProcessorSuccessResult newsProcessorSuccessResult)
    {
        var processingDurationTime = newsProcessorSuccessResult.ProcessingDurationTime;
        var formattedProcessingDurationTime =
            $"{processingDurationTime.Hours:D2}:" +
            $"{processingDurationTime.Minutes:D2}:" +
            $"{processingDurationTime.Seconds:D2}." +
            $"{processingDurationTime.Milliseconds / 10:D2}";

        var message =
            $"🚀Успешно обработана новость \"{newsProcessorSuccessResult.NewsAnalyze.Brief}\" из источника {newsProcessorSuccessResult.NewsUrl}\r\n" +
            $"По новости сформулировано {newsProcessorSuccessResult.NewsAnalyze.Hypotheses.Count} гипотез\r\n" +
            $"Потрачено токенов {newsProcessorSuccessResult.UsageTotalTokenCount}, время обработки {formattedProcessingDurationTime}\r\n" +
            $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss (UTC)}";

        return new AdminSignalMessage(message);
    }

    public AdminSignalMessage Create(NewsProcessorErrorResult newsProcessorErrorResult)
    {
        return new AdminSignalMessage("Test admin failed signal");
    }
}