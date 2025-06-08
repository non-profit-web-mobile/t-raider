using Worker.Schedule;
using Model.Kafka;
using Model.Kafka.Messages;

namespace Worker.Tasks;

public class DemoNewsTask(
	IServiceProvider serviceProvider,
	ILogger<DemoNewsTask> logger)
	: ScopedBackgroundTask(serviceProvider, TaskSchedule.Periodical(TimeSpan.FromSeconds(30)), logger)
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

	protected override async Task ExecuteIterationAsync(IServiceScope serviceScope, CancellationToken cancellationToken)
	{
		var kafkaProducer = serviceScope.ServiceProvider.GetRequiredService<IKafkaProducer>();
		var topicInfoProvider = serviceScope.ServiceProvider.GetRequiredService<ITopicInfoProvider>();
		var kafkaMessageSerializer = serviceScope.ServiceProvider.GetRequiredService<IKafkaMessageSerializer>();

		var random = new Random();
		var randomNewsLink = NewsLinks[random.Next(NewsLinks.Length)];
		var rawNewsMessage = new RawNewsMessage(randomNewsLink, random.Next(1, 11));

		var topicInfo = topicInfoProvider.GetRawNewsTopicInfo();
		var serializedMessage = kafkaMessageSerializer.Serialize(rawNewsMessage);

		await kafkaProducer.ProduceAsync(topicInfo, Guid.NewGuid().ToString(), serializedMessage, cancellationToken);
	}
}