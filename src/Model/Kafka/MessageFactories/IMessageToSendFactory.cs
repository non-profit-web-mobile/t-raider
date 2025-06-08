using Model.Domain;
using Model.Kafka.Messages;

namespace Model.Kafka.MessageFactories;

public interface IMessageToSendFactory
{
    public MessageToSend Create(IReadOnlyList<long> telegramIds, NewsAnalyze newsAnalyze);
    
    public MessageToSend Create(IReadOnlyList<long> telegramIds, IReadOnlyList<NewsAnalyze> news);
}