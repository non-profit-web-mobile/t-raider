using Model.Domain;
using Model.Kafka.Messages;
using Newtonsoft.Json;

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

        var json = JsonConvert.SerializeObject(newsProcessorSuccessResult.NewsAnalyze, Formatting.Indented);
        
        var message =
            $"🚀 Успешно обработана новость \"{newsProcessorSuccessResult.NewsAnalyze.Brief}\" из источника {newsProcessorSuccessResult.NewsUrl}\r\n" +
            $"По новости сформулировано {newsProcessorSuccessResult.NewsAnalyze.Hypotheses.Count} гипотез\r\n" +
            $"Потрачено токенов {newsProcessorSuccessResult.UsageTotalTokenCount}, время обработки {formattedProcessingDurationTime}\r\n" +
            $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss (UTC)}\r\n" +
            $"Ответ openai:\r\n" +
            $"```JSON{json}```";

        return new AdminSignalMessage(message);
    }

    public AdminSignalMessage Create(NewsProcessorErrorResult newsProcessorErrorResult)
    {
        var trimmedErrorMessage = newsProcessorErrorResult.ErrorMessage.Length > 300
            ? newsProcessorErrorResult.ErrorMessage + "..."
            : newsProcessorErrorResult.ErrorMessage;

        return new AdminSignalMessage(
            $"💔 Произошла ошибка при обработке новости из источника {newsProcessorErrorResult.NewsUrl}\r\n" +
            $"📊 Детали ниже:\r\n" +
            $"{trimmedErrorMessage}" +
            $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss (UTC)}");
    }

    public AdminSignalMessage Create(MessageClick messageClick)
    {
        return new AdminSignalMessage(
            $"👀 Получили клик \"{messageClick.ClickType}\" от пользователя для идеи \"{messageClick.Tactics}\" по ссылке {messageClick.Link}\r\n" +
            $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss (UTC)}");
    }
}