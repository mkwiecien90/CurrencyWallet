#region Usings

using CurrencyWallet.Domain.Exceptions;
using CurrencyWallet.Domain.ValueObjects;

#endregion

namespace CurrencyWallet.Domain.Entities
{
    [Serializable]
    public class Wallet
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public IList<WalletBalance> Balances { get; set; } = new List<WalletBalance>();

        public void Deposit(string currency, decimal amount)
        {
            var balance = Balances.FirstOrDefault(b => b.Money.Currency == currency);
            if (balance != null)
                balance.Money = balance.Money.Add(new Money(amount, currency));
            else
                Balances.Add(new WalletBalance(new Money(amount, currency)));
        }

        public bool Withdraw(string currency, decimal amount)
        {
            var money = new Money(amount, currency);
            var balance = Balances.FirstOrDefault(b => b.Money.Currency == currency);
            if (balance != null && balance.Money.IsGreaterThan(money))
            {
                balance.Money = balance.Money.Remove(money);
                return true;
            }
            return false;
        }

        public void Convert(decimal amount, string fromCurrency, string toCurrency, IEnumerable<CurrencyRate> currencyRates)
        {
            var fromCurrencyRate = currencyRates.FirstOrDefault(s => s.Code == fromCurrency);
            var toCurrencyRate = currencyRates.FirstOrDefault(s => s.Code == toCurrency);

            if (fromCurrencyRate is null || toCurrencyRate is null)
            {
                throw new BadRequestException("Rates for one of the currencies are not available.");
            }

            var balance = Balances.FirstOrDefault(c => c.Money.Currency == fromCurrency);

            if (balance is null)
            {
                throw new BadRequestException($"The {fromCurrency} currency is not supported by this wallet.");
            }

            if (!balance.Money.IsGreaterThan(new Money(amount, fromCurrency)))
            {
                throw new BadRequestException($"There are not enough funds in your wallet to change your currency.");
            }

            UpdateBalance(fromCurrency, amount, (money) => balance.Money.Remove(money));

            decimal amountInPLN = amount / fromCurrencyRate.Mid;

            decimal convertedAmount = amountInPLN * toCurrencyRate.Mid;

            UpdateBalance(toCurrency, convertedAmount, (money) => money.Add(new Money(convertedAmount, toCurrency)));
        }

        private void UpdateBalance(string currency, decimal amount, Func<Money, Money> updateFunc)
        {
            var balance = Balances.FirstOrDefault(c => c.Money.Currency == currency);

            if (balance is null)
            {
                balance = new WalletBalance(new Money(amount, currency));
                Balances.Add(balance);
                return;
            }

            var money = new Money(amount, currency);
            balance.Money = updateFunc(money);
        }
    }
}
