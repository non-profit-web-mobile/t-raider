using Model.Domain;
using Model.Kafka.Messages;

namespace Model.Kafka.MessageFactories;

public interface IHypothesesMessageMessageFactory
{
    HypothesesMessage Create(NewsProcessorSuccessResult newsProcessorSuccessResult);
}