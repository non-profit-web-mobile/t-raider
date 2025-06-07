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

		var item = new Item { Value = request.Value };
		await dataExecutionContext.ExecuteWithTransactionAsync(
			async repositories => await repositories.Items.CreateAsync(item, cancellationToken), 
			IsolationLevel.Snapshot,
			cancellationToken);

		return new CreateItemResponse(true);
	}
}