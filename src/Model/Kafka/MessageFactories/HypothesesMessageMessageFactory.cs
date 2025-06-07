using Model.Domain;
using Model.Kafka.Messages;

namespace Model.Kafka.MessageFactories;

public class HypothesesMessageMessageFactory : IHypothesesMessageMessageFactory
{
    public HypothesesMessage Create(NewsProcessorSuccessResult newsProcessorSuccessResult)
    {
        return new HypothesesMessage(newsProcessorSuccessResult.newsAnalyze);
    }
}