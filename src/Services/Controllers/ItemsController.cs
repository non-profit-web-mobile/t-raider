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
	public async Task<ActionResult<CreateItemResponse>> CreateItemAsync(
		[FromBody] CreateItemRequest request,
		CancellationToken cancellationToken)
	{
		var topicInfo = topicInfoProvider.GetItemsTopicInfo();
		var key = Guid.NewGuid().ToString();
		var message = new ItemMessage(request.Value);
		var serializedMessage = kafkaMessageSerializer.Serialize(message);
		await kafkaProducer.ProduceAsync(topicInfo, key, serializedMessage);

		var userProfile = new UserProfile
		{
			TelegramId = 1,
			StreamEnabled = true,
			SummaryEnabled = true,
			Confidence = 1,
			Tickers = new List<Ticker>
			{
				new()
				{
					Symbol = "AAPL"
				}
			}
		};

		await dataExecutionContext.ExecuteWithTransactionAsync(
			async repositories => await repositories.UserProfileRepository.CreateAsync(userProfile, cancellationToken), 
			IsolationLevel.Snapshot,
			cancellationToken);

		var existingUserProfile = await dataExecutionContext.ExecuteAsync(
			async repositories => await repositories.UserProfileRepository.GetByIdAsync(userProfile.Id, cancellationToken),
			cancellationToken);

		return new CreateItemResponse(true);
	}
}