namespace Model.Domain;

public record Hypothesis(
    IReadOnlyList<TickerInfo> Tickers,
    ActionType Action,
    double Probability,
    int Period,
    string Tactics,
    string EntryEvent,
    decimal StopLoss,
    decimal TakeProfit
);