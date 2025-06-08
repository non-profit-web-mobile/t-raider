using System.Collections.Generic;
using Model.Domain;
using Model.Kafka.Messages;

namespace Model.Kafka.MessageFactories;

public interface IMessageToSendFactory
{
	public MessageToSend Create(IReadOnlyList<long> telegramIds, NewsAnalyze newsAnalyze);

	public MessageToSend Create(IReadOnlyList<long> telegramIds, IReadOnlyList<NewsAnalyze> news);
}

public class MessageToSendFactory : IMessageToSendFactory
{
	public MessageToSend Create(IReadOnlyList<long> telegramIds, NewsAnalyze newsAnalyze)
	{
		if (newsAnalyze.Hypotheses.Count == 0)
		{
			return new MessageToSend(
				telegramIds,
				"❗️ Нет торговых идей по данной новости.",
				new List<Button> { new Button("Открыть новость", newsAnalyze.SourceUrl.ToString()) }
			);
		}

		var blocks = newsAnalyze.Hypotheses.Select(FormatHypothesisBlock).ToList();
		var message = FormatFinalMessage(blocks, newsAnalyze);

		var firstHypothesis = newsAnalyze.Hypotheses.First();
		var tbankUrl = $"https://www.tbank.ru/invest/stocks/{firstHypothesis.Ticker}/";
		var buttons = new List<Button>
		{
			new Button("Открыть новость", newsAnalyze.SourceUrl.ToString()),
			new Button($"T-Инвестиции: {firstHypothesis.Ticker}", tbankUrl)
		};

		return new MessageToSend(
			telegramIds,
			message,
			buttons
		);
	}

	public MessageToSend Create(IReadOnlyList<long> telegramIds, IReadOnlyList<NewsAnalyze> news)
	{
		if (news == null || news.Count == 0)
		{
			return new MessageToSend(
				telegramIds,
				"❗️ Нет торговых идей за выбранный период.",
				new List<Button>()
			);
		}

		var header = "📊 Сводка с топовыми гипотезами по инвестированию 👇\n\n";
		var blocks = news.Select(n => FormatNewsBlock(n)).ToList();
		var message = header + string.Join("\n\n___\n\n", blocks);

		var buttons = new List<Button> { };

		return new MessageToSend(
			telegramIds,
			message,
			buttons
		);
	}

	private static string FormatHypothesisBlock(Hypothesis hypothesis)
	{
		return string.Join("\n", new[]
		{
			FormatActionLine(hypothesis),
			FormatPriceLine(hypothesis),
			FormatStopLossLine(hypothesis),
			FormatTakeProfitLine(hypothesis),
			FormatPeriodLine(hypothesis),
			FormatTacticsLine(hypothesis),
			FormatProbabilityLine(hypothesis)
		});
	}

	private static string FormatActionLine(Hypothesis hypothesis)
	{
		return $"**{MapActionToText(hypothesis.Action)} {hypothesis.Ticker}**";
	}

	private static string FormatPriceLine(Hypothesis hypothesis)
	{
		return !string.IsNullOrWhiteSpace(hypothesis.Price.ToString("F0"))
			? $"  💰 Текущая цена: {hypothesis.Price}₽"
			: string.Empty;
	}

	private static string FormatStopLossLine(Hypothesis hypothesis)
	{
		return !string.IsNullOrWhiteSpace(hypothesis.StopLoss.ToString("F0"))
			? $"  ⛔️ Стоп-лосс: {hypothesis.StopLoss}₽"
			: string.Empty;
	}

	private static string FormatTakeProfitLine(Hypothesis hypothesis)
	{
		return !string.IsNullOrWhiteSpace(hypothesis.TakeProfit.ToString("F0"))
			? $"  🎯 Тейк-профит: {hypothesis.TakeProfit}₽"
			: string.Empty;
	}

	private static string FormatPeriodLine(Hypothesis hypothesis)
	{
		return !string.IsNullOrWhiteSpace(hypothesis.Period.ToString("F0"))
			? $"  ⏳ Срок актуальности: {hypothesis.Period} часов"
			: string.Empty;
	}

	private static string FormatTacticsLine(Hypothesis hypothesis)
	{
		return $"💡 Идея: {hypothesis.Tactics}";
	}

	private static string FormatProbabilityLine(Hypothesis hypothesis)
	{
		return $"📈 Вероятность: {(hypothesis.Probability * 100):F0}%";
	}

	private static string FormatEventLine(NewsAnalyze newsAnalyze)
	{
		return $"Событие: {newsAnalyze.Brief}";
	}

	private static string FormatFinalMessage(List<string> blocks, NewsAnalyze newsAnalyze)
	{
		return string.Join("___", blocks) + FormatEventLine(newsAnalyze);
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

	private static string FormatNewsBlock(NewsAnalyze news)
	{
		var header = $"📰 {news.Brief}\n";
		var hypothesesBlocks = news.Hypotheses.Select(FormatHypothesisBlock);
		var hypothesesText = string.Join("\n\n", hypothesesBlocks);
		return header + hypothesesText;
	}
}