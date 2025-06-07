using Microsoft.AspNetCore.Mvc;
using Model.Kafka;
using Model.Kafka.Messages;

namespace Services.Controllers;

[Route("items")]
public class ItemsController(
	ITopicInfoProvider topicInfoProvider,
	IKafkaMessageSerializer kafkaMessageSerializer,
	IKafkaProducer kafkaProducer)
	: ControllerBase
{
	[HttpPost]
	public async Task<ActionResult<CreateItemResponse>> CreateItemAsync(
		[FromBody] CreateItemRequest request,
		CancellationToken cancellationToken)
	{
		var topicInfo = topicInfoProvider.GetRawNewsTopicInfo();
		var key = Guid.NewGuid().ToString();
		var message = new RawNewsMessage(request.Value);
		var serializedMessage = kafkaMessageSerializer.Serialize(message);
		await kafkaProducer.ProduceAsync(topicInfo, key, serializedMessage, cancellationToken);

		return new CreateItemResponse(true);
	}
}