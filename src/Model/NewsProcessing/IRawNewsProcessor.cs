using Model.Kafka.Messages;

namespace Model.NewsProcessing;

public interface IRawNewsProcessor
{
    Task ProcessAsync(RawNewsMessage rawNewsMessage);
}