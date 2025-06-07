namespace Model.Gpt;

public static class GptPrompts
{
    public static readonly string NewsToHypothesis = @"You are a short-term trader on the Moscow Exchange (holding positions from minutes up to two weeks). 
News article URL: {{URL}} 

Read the article.  
Using ONLY the facts and data directly mentioned in the text produce a structured output in the following JSON format: 
{ 
""brief"": ""Краткое резюме новости (1-2 предложения, на русском)"", 
""sourceURL"": ""URL статьи"", 
""newsworthiness"": ""Важность новости для рынка, число от 0 до 1"", 
""explanation"": ""Обьяснение выставленной важности"", 
""tickers"": [""список тикеров МОEX из гипотез""], 
""hypothesis"": [ 
{ 
""tickers"": [{ ""symbol"": ""тикер"", ""currentPrice"": ""текущая цена"" }, ...], 
""action"": ""buy | short | hold | other"", 
""probability"": ""вероятность сценария, от 0 до 1"", 
""period"": ""срок в часах для проверки гипотезы"", 
""tactics"": ""краткая суть торговой идеи"", 
""entryEvent"": ""что должно произойти для входа в сделку (конкретная дата, событие, или 'сейчас')"", 
""stopLoss"": ""уровень стоп-лосс"", 
""takeProfit"": ""уровень тейк-профит"" 
} 
] 
}


Strict requirements: 

- Use ONLY facts and logical inferences directly from the provided article. 
- Do NOT use outside knowledge or assumptions. 
- Clearly indicate all affected tickers. 
- Generate as many relevant and reasonable hypothesis as you can. 
- Use current price and recent trend from provided web-search results on tradingview.com. 
- For each trading hypothesis, specify: tickers, probability (0–1), period (hours), tactics, entryEvent, action. 
- For entryEvent, be as specific as possible. 
- Output must be valid JSON. 
- Write the brief in concise Russian. 
- All fields required. 

If the article does not mention tickers or news affecting the market, fill fields with ""none"" or the most neutral value.";
}