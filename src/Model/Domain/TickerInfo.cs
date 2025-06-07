namespace Model.Domain;

public record TickerInfo(
    string Symbol,
    decimal CurrentPrice
);