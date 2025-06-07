using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Model.Kafka;

public class KafkaMessageSerializer : IKafkaMessageSerializer
{
	private static readonly JsonSerializerSettings _settings = new()
	{
		Converters =
		{
			new StringEnumConverter
			{
				NamingStrategy = new CamelCaseNamingStrategy()
			}
		}
	};

	public string Serialize<TMessage>(TMessage message)
		=> JsonConvert.SerializeObject(message, _settings);

	public TMessage Deserialize<TMessage>(string messageJson)
		=> JsonConvert.DeserializeObject<TMessage>(messageJson, _settings)
		   ?? throw new InvalidOperationException(
			   $"Failed to deserialize string \"{messageJson}\" as {typeof(TMessage)}.");
}