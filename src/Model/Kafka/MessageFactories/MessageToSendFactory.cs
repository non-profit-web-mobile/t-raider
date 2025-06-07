using System.Collections.Generic;
using Model.Domain;
using Model.Kafka.Messages;

namespace Model.Kafka.MessageFactories;

public interface IMessageToSendFactory
{
	public MessageToSend Create(IReadOnlyList<long> telegramIds, NewsAnalyze newsAnalyze);
}

public class MessageToSendFactory : IMessageToSendFactory
{
	public MessageToSend Create(IReadOnlyList<long> telegramIds, NewsAnalyze newsAnalyze)
	{
		if (newsAnalyze.Hypotheses.Count == 0)
		{
			return new MessageToSend(
				new List<long>(),
				"❗️ Нет торговых идей по данной новости.",
				new List<Button> { new Button("Открыть новость", newsAnalyze.SourceUrl.ToString()) }
			);
		}

		var blocks = newsAnalyze.Hypotheses.Select(FormatHypothesisBlock).ToList();
		var message = FormatFinalMessage(blocks, newsAnalyze);

		var buttons = new List<Button>
		{
			new Button("Открыть новость", newsAnalyze.SourceUrl.ToString())
		};

		return new MessageToSend(
			telegramIds,
			message,
			buttons
		);
	}

	private static string FormatHypothesisBlock(Hypothesis hypothesis)
	{
		var actionText = MapActionToText(hypothesis.Action);
		var stopLossTakeProfit = hypothesis.Action != "Hold"
			? $"⛔️ Стоп-лосс: {hypothesis.StopLoss}\r\n" +
			  $"🎯 Тейк-профит: {hypothesis.TakeProfit}\r\n"
			: string.Empty;
		
		return $"{actionText} {hypothesis.Ticker}\r\n" +
		       stopLossTakeProfit +
		       $"⏳ Срок актуальности: {hypothesis.Period}\r\n\r\n" +
		       $"💡 Идея: {hypothesis.Tactics}\r\n" +
		       $"📈 Вероятность: 60%\r\n";
	}

	private static string FormatFinalMessage(List<string> blocks, NewsAnalyze newsAnalyze)
	{
		return string.Join("\r\n", blocks) +
		       $"📰 Событие: [{newsAnalyze.Brief}]({newsAnalyze.SourceUrl})";
	}

	private static string MapActionToText(string action)
	{
		return action switch
		{
			"Long" => "🟢 Покупай",
			"Short" => "🔴 Продавай",
			"Hold" => "🟡 Держи",
			_ => ""
		};
	}
}