using Model.Domain;
using Model.Kafka.Messages;

namespace Model.Kafka.MessageFactories;

public interface IAdminSignalMessageFactory
{
    AdminSignalMessage Create(NewsProcessorSuccessResult newsProcessorSuccessResult);
    AdminSignalMessage Create(NewsProcessorErrorResult newsProcessorErrorResult);
}