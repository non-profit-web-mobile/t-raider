using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using Polly.Contrib.WaitAndRetry;
using Polly.Wrap;

namespace Model.Kafka;

public class KafkaProducer(ILogger<KafkaProducer> logger) : IKafkaProducer
{
	private readonly Dictionary<string, IProducer<string, string>> _producers = new();
	private readonly Lock _lock = new();
	private bool _disposed;

	public async Task ProduceAsync(TopicInfo topicInfo, string key, string message)
	{
		var asyncPolicy = GetAsyncPolicy();
		var producer = GetOrCreateProducer(topicInfo.BootstrapServers);

		try
		{
			await asyncPolicy.ExecuteAsync(async () =>
			{
				var kafkaMessage = new Message<string, string>
				{
					Key = key,
					Value = message,
					Timestamp = new Timestamp(DateTime.UtcNow)
				};

				var deliveryResult = await producer.ProduceAsync(topicInfo.TopicName, kafkaMessage);
				
				if (deliveryResult.Status != PersistenceStatus.Persisted)
					throw new InvalidOperationException($"Message was not persisted. Status: {deliveryResult.Status}.");
			});
		}
		catch (BrokenCircuitException exception)
		{
			logger.LogError(
				exception,
				"Kafka circuit breaker is open - unable to produce message to topic '{TopicName}' with key '{Key}'.",
				topicInfo.TopicName,
				key);

			throw new InvalidOperationException("Kafka service is temporarily unavailable.", exception);
		}
		catch (Exception exception)
		{
			logger.LogError(
				exception,
				"Failed to produce message to topic '{TopicName}' with key '{Key}' after all retry attempts.",
				topicInfo.TopicName,
				key);

			throw;
		}
	}

	private IProducer<string, string> GetOrCreateProducer(string bootstrapServers)
	{
		lock (_lock)
		{
			if (_producers.TryGetValue(bootstrapServers, out var producer))
				return producer;

			var producerConfig = new ProducerConfig
			{
				BootstrapServers = bootstrapServers,
				Acks = Acks.All,
				EnableIdempotence = true,
				MaxInFlight = 5,
				LingerMs = 5,
				BatchSize = 16384,
				CompressionType = CompressionType.Snappy,
				MessageTimeoutMs = 30000,
				RequestTimeoutMs = 30000,
				MessageSendMaxRetries = 3,
				RetryBackoffMs = 100,
				SecurityProtocol = SecurityProtocol.Plaintext
			};

			producer = new ProducerBuilder<string, string>(producerConfig)
				.SetErrorHandler((_, error) =>
				{
					logger.LogError("Kafka producer error: {ErrorCode} - {ErrorReason}.", error.Code, error.Reason);
				})
				.SetLogHandler((_, logMessage) =>
				{
					logger.Log(LogLevel.Information, "Kafka producer message: {Message}.", logMessage.Message);
				})
				.Build();

			_producers[bootstrapServers] = producer;

			return producer;
		}
	}

	public void Dispose()
	{
		if (_disposed)
			return;

		lock (_lock)
		{
			if (_disposed)
				return;

			foreach (var producer in _producers.Values)
			{
				producer.Flush(TimeSpan.FromSeconds(10));
				producer.Dispose();
			}

			_producers.Clear();
			_disposed = true;
		}
	}

	private static AsyncPolicyWrap GetAsyncPolicy()
	{
		var circuitBreakerPolicy = Policy
			.Handle<Exception>()
			.CircuitBreakerAsync(
				exceptionsAllowedBeforeBreaking: 5,
				durationOfBreak: TimeSpan.FromMinutes(1));

		var retryPolicy = Policy
			.Handle<ProduceException<string, string>>()
			.Or<KafkaException>()
			.Or<TimeoutException>()
			.WaitAndRetryAsync(
				Backoff.DecorrelatedJitterBackoffV2(
					medianFirstRetryDelay: TimeSpan.FromMilliseconds(100),
					retryCount: 5));

		return Policy.WrapAsync(circuitBreakerPolicy, retryPolicy);
	}
}