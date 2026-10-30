namespace OrderModels.Anemic;

public record Money
{
    public decimal Amount { get; }
    public string Currency { get; }

    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public static Money Create(decimal amount, string currency)
    {
        return new Money(amount, currency.ToUpperInvariant());
    }

    public Money Add(Money other)
    {
        return new Money(Amount + other.Amount, Currency);
    }

    public Money Multiply(int quantity)
    {
        return new Money(Amount * quantity, Currency);
    }
}
