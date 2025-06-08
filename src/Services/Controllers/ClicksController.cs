using Microsoft.AspNetCore.Mvc;
using Model.Domain;
using Model.Kafka;
using Model.Kafka.MessageFactories;
using Model.MessageClicks;

namespace Services.Controllers;

public class ClicksController(
	ILogger<ClicksController> logger,
	IMessageClickEncoder messageClickEncoder,
	IAdminSignalMessageFactory adminSignalMessageFactory,
	ITopicInfoProvider topicInfoProvider,
	IKafkaProducer kafkaProducer,
	IKafkaMessageSerializer kafkaMessageSerializer) : Controller
{
	[HttpGet]
	[Route("test")]
	public IActionResult Test()
	{
		return Ok();
	}

	[HttpGet]
	[Route("c")]
	public async Task RedirectAsync(
		[FromQuery(Name = "m")] string messageKey,
		[FromQuery(Name = "h")] string antiForgeryHash, // this hash needs to be validated in future for anti-forgery
		[FromQuery(Name = "u")] string redirectUri,
		CancellationToken cancellationToken)
	{
		await RedirectAsyncInternal(HttpContext.Response, redirectUri);

		await PublishUserClickSignalAsync(messageKey, redirectUri, cancellationToken);
	}

	private static async Task RedirectAsyncInternal(HttpResponse httpContextResponse, string redirectUri)
	{
		httpContextResponse.Redirect(location: redirectUri, permanent: false);
		await httpContextResponse.CompleteAsync();
	}

	private async Task PublishUserClickSignalAsync(
		string messageKey,
		string redirectUri,
		CancellationToken cancellationToken)
	{
		try
		{
			await PublishUserClickSignalSafeAsync(messageKey, redirectUri, cancellationToken);
		}
		catch (Exception exception)
		{
			logger.LogError(exception, "Failed to publish user click signal");
		}
	}

	private async Task PublishUserClickSignalSafeAsync(
		string messageKey,
		string redirectUri,
		CancellationToken cancellationToken)
	{
		var topicInfo = topicInfoProvider.GetAdminSignalsTopicInfo();
		var key = Guid.NewGuid().ToString();

		var messageClick = messageClickEncoder.Decode(messageKey, redirectUri);
		var message = adminSignalMessageFactory.Create(messageClick);

		var serializedMessage = kafkaMessageSerializer.Serialize(message);

		await kafkaProducer.ProduceAsync(topicInfo, key, serializedMessage, cancellationToken);
	}
}