namespace Model.Kafka;

public interface IKafkaMessageSerializer
{
	string Serialize<TMessage>(TMessage message);

	TMessage Deserialize<TMessage>(string messageJson);
}