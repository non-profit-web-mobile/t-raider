using Worker.Schedule;
using Model.Kafka;
using Model.Kafka.Messages;

namespace Worker.Tasks;

public class DemoNewsTask(
	IServiceProvider serviceProvider,
	ILogger<DemoNewsTask> logger)
	: BackgroundTask(TaskSchedule.Periodical(TimeSpan.FromMinutes(10)), logger)
{
	private static readonly RawNewsMessage[] NewsLinks =
	[
		new ("https://www.rbc.ru/politics/23/06/2023/6495e7c99a79473ce4258bf6", 9),
		new ("https://panorama.pub/news/v-vasingtone-startovala-ezegodnaa-ceremonia", 0),
		new ("https://www.rbc.ru/finances/06/06/2025/68403e4c9a7947dda98935cf", 9),
		new ("https://www.tbank.ru/invest/social/profile/Interfax/dca05b8d-8c5e-3f2c-b2a1-cda75d693b46",
			7),
		new ("https://www.rbc.ru/quote/news/article/682ec7659a79473670690cb4?from=newsfeed",
			7),
		new("https://bcs-express.ru/novosti-i-analitika/o-torgakh-aktsiiami-x5-na-mosbirzhe", 7),
		new("https://www.rbc.ru/quote/news/article/6762def09a79479ffc4b681d", 9),
		new("https://www.rbc.ru/quote/news/article/6345719e9a79472282efb0b9", 9)
	];

	protected override async Task ExecuteIterationAsync(CancellationToken cancellationToken)
	{
		var kafkaProducer = serviceProvider.GetRequiredService<IKafkaProducer>();
		var topicInfoProvider = serviceProvider.GetRequiredService<ITopicInfoProvider>();
		var kafkaMessageSerializer = serviceProvider.GetRequiredService<IKafkaMessageSerializer>();

		var random = new Random();
		var randomNews = NewsLinks[random.Next(NewsLinks.Length)];
		var topicInfo = topicInfoProvider.GetRawNewsTopicInfo();
		var serializedMessage = kafkaMessageSerializer.Serialize(randomNews);

		await kafkaProducer.ProduceAsync(topicInfo, Guid.NewGuid().ToString(), serializedMessage, cancellationToken);
	}
}