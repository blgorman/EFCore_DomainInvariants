namespace OrderModels.Domain;

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
        //TODO: Talk Step 4 — Uncomment the two guard clauses below
        //if (amount < 0) throw new ArgumentException("Amount cannot be negative.", nameof(amount));
        //if (string.IsNullOrWhiteSpace(currency)) throw new ArgumentException("Currency cannot be empty.", nameof(currency));

        return new Money(amount, currency.ToUpperInvariant());
    }

    public Money Add(Money other)
    {
        //TODO: Talk Step 4 — Uncomment the currency guard clause below
        //if (other.Currency != Currency) throw new InvalidOperationException($"Cannot add {other.Currency} to {Currency}: currency mismatch.");

        return new Money(Amount + other.Amount, Currency);
    }

    public Money Multiply(int quantity)
    {
        return new Money(Amount * quantity, Currency);
    }

    public override string ToString() => $"{Amount} {Currency}";
}
