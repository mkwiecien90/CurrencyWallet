namespace CurrencyWallet.Domain.ValueObjects
{
    public class Money
    {
        public decimal Amount { get; }
        public string Currency { get; }

        public Money(decimal amount, string currency)
        {
            if (amount < 0)
                throw new ArgumentException("Amount cannot be negative.");

            Amount = amount;
            Currency = currency ?? throw new ArgumentNullException(nameof(currency));
        }

        public Money Add(Money money)
        {
            if (Currency != money.Currency)
                throw new InvalidOperationException("Cannot add amounts with different currencies.");

            return new Money(Amount + money.Amount, Currency);
        }

        public Money Remove(Money money)
        {
            if (Currency != money.Currency)
                throw new InvalidOperationException("Cannot remove amounts with different currencies.");

            return new Money(Amount - money.Amount, Currency);
        }

        public bool IsGreaterThan(Money money)
        {
            if (Currency != money.Currency)
                throw new InvalidOperationException("Cannot compare amounts with different currencies.");

            return Amount > money.Amount;
        }
    }
}
