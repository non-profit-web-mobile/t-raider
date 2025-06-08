namespace Model.Kafka.Messages;

[DataContract]
public record Button(
	[property: DataMember(Name = "text")] string Text,
	[property: DataMember(Name = "url")] string Url);

[DataContract]
public record MessageToSend(
	[property: DataMember(Name = "telegramIds")] IReadOnlyList<long> TelegramIds,
	[property: DataMember(Name = "message")] string Message,
	[property: DataMember(Name = "buttons")] IReadOnlyList<Button> Buttons);