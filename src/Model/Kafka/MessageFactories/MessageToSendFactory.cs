using System.Web;
using Model.Domain;
using Model.Kafka.Messages;
using Model.MessageClicks;

namespace Model.Kafka.MessageFactories;

public class MessageToSendFactory(IMessageClickEncoder messageClickEncoder) : IMessageToSendFactory
{
    public MessageToSend Create(IReadOnlyList<long> telegramIds, NewsAnalyze newsAnalyze)
    {
        var hypotheses = newsAnalyze.Hypotheses.Take(1).ToList();

        if (hypotheses.Count == 0)
        {
            return new MessageToSend(
                telegramIds,
                "❗️ Нет торговых идей по данной новости.",
                new List<Button> { new("Открыть новость", newsAnalyze.SourceUrl.ToString()) }
            );
        }

        var blocks = hypotheses.Select(FormatHypothesisBlock).ToList();
        var message = FormatFinalMessage(blocks, newsAnalyze);

        var hypothesis = hypotheses[0];
        var hypothesisTactics = hypothesis.Tactics;
        var hypothesisTicker = hypothesis.Ticker;

        var toTickerUrl = $"https://www.tbank.ru/invest/stocks/{hypothesisTicker}/";
        var toTickerMessageKey = messageClickEncoder.Encode(
            clickType: "OnTicker", 
            tactics: hypothesisTactics);
        var toTickerDecoratedUrl = DecorateForTrackingLink(
            messageKey: toTickerMessageKey, 
            antiForgeryHash: Guid.NewGuid().ToString("N"), 
            link: toTickerUrl);

        var toNewsUrl = newsAnalyze.SourceUrl.ToString();
        var toNewsUrlKey = messageClickEncoder.Encode(
            clickType: "OnNews", 
            tactics: hypothesisTactics);
        var toNewsDecoratedUrl = DecorateForTrackingLink(
            messageKey: toNewsUrlKey, 
            antiForgeryHash: Guid.NewGuid().ToString("N"), 
            link: toNewsUrl);

        var buttons = new List<Button>
        {
            new("Открыть новость", toNewsDecoratedUrl),
            new($"{hypothesisTicker} в T-Инвестициях", toTickerDecoratedUrl)
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

    private static string DecorateForTrackingLink(string messageKey, string antiForgeryHash, string link)
    {
        return $"https://d5dr810i1jcpfspl7aun.pdkwbi1w.apigw.yandexcloud.net/services/" +
               $"c?" +
               $"m={HttpUtility.UrlEncode(messageKey)}&" +
               $"h={HttpUtility.UrlEncode(antiForgeryHash)}&" +
               $"u={HttpUtility.UrlEncode(link)}";
    }

    private static string FormatHypothesisBlock(Hypothesis hypothesis)
    {
        return string.Join("\r\n", new[]
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
        return $"{MapActionToText(hypothesis.Action)} {hypothesis.Ticker}**";
    }

    private static string FormatPriceLine(Hypothesis hypothesis)
    {
        return !string.IsNullOrWhiteSpace(hypothesis.Price.ToString("F0"))
            ? $"- 💰 Текущая цена: {hypothesis.Price}₽"
            : string.Empty;
    }

    private static string FormatStopLossLine(Hypothesis hypothesis)
    {
        return !string.IsNullOrWhiteSpace(hypothesis.StopLoss.ToString("F0"))
            ? $"- ⛔️ Стоп-лосс: {hypothesis.StopLoss}₽"
            : string.Empty;
    }

    private static string FormatTakeProfitLine(Hypothesis hypothesis)
    {
        return !string.IsNullOrWhiteSpace(hypothesis.TakeProfit.ToString("F0"))
            ? $"- 🎯 Тейк-профит: {hypothesis.TakeProfit}₽"
            : string.Empty;
    }

    private static string FormatPeriodLine(Hypothesis hypothesis)
    {
        return !string.IsNullOrWhiteSpace(hypothesis.Period.ToString("F0"))
            ? $"- ⏳ Срок актуальности: {hypothesis.Period} часов"
            : string.Empty;
    }

    private static string FormatTacticsLine(Hypothesis hypothesis)
    {
        return $"💡 **Идея**: {hypothesis.Tactics}";
    }

    private static string FormatProbabilityLine(Hypothesis hypothesis)
    {
        return $"📈 **Вероятность**: {(hypothesis.Probability * 100):F0}%";
    }

    private static string FormatEventLine(NewsAnalyze newsAnalyze)
    {
        return $"\r\n**Событие**: {newsAnalyze.Brief}";
    }

    private static string FormatFinalMessage(List<string> blocks, NewsAnalyze newsAnalyze)
    {
        return string.Join("\r\n___\r\n", blocks) + FormatEventLine(newsAnalyze);
    }

	private static string MapActionToText(string action)
	{
		return action switch
		{
			"Long" => "🟢 **Покупай",
			"Short" => "🔴 **Продавай",
			"Hold" => "🟡 **Держи",
			_ => "**"
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