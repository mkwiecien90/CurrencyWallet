#region Usings

using System.Text.Json.Serialization;
using MediatR;

#endregion

namespace CurrencyWallet.Application.Commands
{
    public class ConvertCurrencyCommand : IRequest<bool>
    {
        [JsonIgnore]
        public Guid WalletId { get; }
        public string FromCurrency { get; }
        public string ToCurrency { get; }
        public decimal Amount { get; }

        public ConvertCurrencyCommand(Guid walletId, string fromCurrency, string toCurrency, decimal amount)
        {
            WalletId = walletId;
            FromCurrency = fromCurrency;
            ToCurrency = toCurrency;
            Amount = amount;
        }
    }
}
