using System.Runtime.Serialization;
using Model.Domain;

namespace Model.Kafka.Messages;

[DataContract]
public record HypothesesForUsersMessage(
	[property: DataMember(Name = "telegramIds")] IReadOnlyList<long> TelegramIds,
	[property: DataMember(Name = "newsAnalyze")] NewsAnalyze NewsAnalyze);