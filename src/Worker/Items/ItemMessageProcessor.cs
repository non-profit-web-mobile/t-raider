using Confluent.Kafka;
using Model.Data;
using Model.Domain;
using Model.Kafka;
using Model.Kafka.Messages;
using IsolationLevel = System.Data.IsolationLevel;

namespace Worker.Items;

public class ItemMessageProcessor(
	IKafkaMessageSerializer kafkaMessageSerializer,
	IDataExecutionContext dataExecutionContext)
	: IKafkaMessageProcessor<ItemMessage>
{
	public ItemMessage Deserialize(ConsumeResult<string, string> consumeResult)
		=> kafkaMessageSerializer.Deserialize<ItemMessage>(consumeResult.Message.Value);

	public async Task ProduceAsync(ItemMessage message, CancellationToken cancellationToken)
	{
		var userProfile = new UserProfile
		{
			TelegramId = 1,
			StreamEnabled = true,
			SummaryEnabled = true,
			Confidence = 1,
			Session = "{}",
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
	}
}