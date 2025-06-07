using System.ComponentModel.DataAnnotations;

namespace Model.Kafka;

public class KafkaOptions
{
	public const string SectionKey = "Kafka";

	[Required]
	public required string BootstrapServers { get; set; }
}