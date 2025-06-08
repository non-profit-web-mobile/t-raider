using Worker.Schedule;
using Model.Kafka;
using Model.Kafka.Messages;

namespace Worker.Tasks;

public class DemoNewsTask(
	IServiceProvider serviceProvider,
	ILogger<DemoNewsTask> logger)
	: BackgroundTask(TaskSchedule.Periodical(TimeSpan.FromMinutes(1)), logger)
{
	private static readonly string[] NewsLinks =
	[
		"https://ria.ru/20210227/investory-1599270790.html",
		"https://ria.ru/20210227/rassledovanie-1599234947.html",
		"https://ria.ru/20210227/riski-1599214473.html",
		"https://ria.ru/20210227/valyuta-1599206472.html",
		"https://ria.ru/20210227/investory-1599270790.html",
		"https://ria.ru/20210227/rassledovanie-1599234947.html",
		"https://ria.ru/20210227/riski-1599214473.html",
		"https://ria.ru/20210227/valyuta-1599206472.html",
		"https://ria.ru/20210227/kreditki-1599202110.html",
		"https://ria.ru/20210227/gosdolg-1599197744.html",
		"https://ria.ru/20210227/teplo-1599197580.html"
	];

	protected override async Task ExecuteIterationAsync(CancellationToken cancellationToken)
	{
		var kafkaProducer = serviceProvider.GetRequiredService<IKafkaProducer>();
		var topicInfoProvider = serviceProvider.GetRequiredService<ITopicInfoProvider>();
		var kafkaMessageSerializer = serviceProvider.GetRequiredService<IKafkaMessageSerializer>();

		var random = new Random();
		var randomNewsLink = NewsLinks[random.Next(NewsLinks.Length)];
		var rawNewsMessage = new RawNewsMessage(randomNewsLink, random.Next(1, 11));

		var topicInfo = topicInfoProvider.GetRawNewsTopicInfo();
		var serializedMessage = kafkaMessageSerializer.Serialize(rawNewsMessage);

		await kafkaProducer.ProduceAsync(topicInfo, Guid.NewGuid().ToString(), serializedMessage, cancellationToken);
	}
}