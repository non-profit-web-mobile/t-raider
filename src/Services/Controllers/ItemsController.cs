using System.Data;
using Microsoft.AspNetCore.Mvc;
using Model.Data;
using Model.Domain;
using Model.Kafka;
using Model.Kafka.Messages;

namespace Services.Controllers;

[Route("items")]
public class ItemsController(
	ITopicInfoProvider topicInfoProvider,
	IKafkaMessageSerializer kafkaMessageSerializer,
	IKafkaProducer kafkaProducer,
	IDataExecutionContext dataExecutionContext)
	: ControllerBase
{
	[HttpPost]
	public async Task<ActionResult<CreateItemResponse>> CreateItemAsync(CancellationToken cancellationToken)
	{
		var userProfile = new UserProfile
		{
			TelegramId = 261582798,
			StreamEnabled = true,
			SummaryEnabled = true,
			Confidence = 0.1,
			Session = "{}",
			Tickers = new List<Ticker>
			{
				new() { Symbol = "SBER" }
			}
		};

		await dataExecutionContext.ExecuteWithTransactionAsync(
			async repositories => await repositories.UserProfileRepository
				.CreateAsync(userProfile, cancellationToken),
			IsolationLevel.Snapshot,
			cancellationToken);

		var topicInfo = topicInfoProvider.GetRawNewsTopicInfo();
		var key = Guid.NewGuid().ToString();
		var message = new RawNewsMessage(
			"https://www.rbc.ru/finances/06/06/2025/68403e4c9a7947dda98935cf",
			10);
		var serializedMessage = kafkaMessageSerializer.Serialize(message);
		await kafkaProducer.ProduceAsync(topicInfo, key, serializedMessage, cancellationToken);

		return new CreateItemResponse(true);
	}
}