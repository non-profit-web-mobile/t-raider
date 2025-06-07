using Model.Domain;
using Model.Kafka.Messages;

namespace Model.Kafka.MessageFactories;

public class AdminSignalMessageFactory : IAdminSignalMessageFactory
{
    public AdminSignalMessage Create(NewsProcessorSuccessResult newsProcessorSuccessResult)
    {
        return new AdminSignalMessage("Test admin success signal");
    }

    public AdminSignalMessage Create(NewsProcessorErrorResult newsProcessorErrorResult)
    {
        return new AdminSignalMessage("Test admin failed signal");
    }
}