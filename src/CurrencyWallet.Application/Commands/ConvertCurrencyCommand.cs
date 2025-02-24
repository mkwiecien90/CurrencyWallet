#region Usings

using System.Text.Json.Serialization;
using CurrencyWallet.Domain.Entities;
using MediatR;

#endregion

namespace CurrencyWallet.Application.Commands
{
    public class ConvertCurrencyCommand : IRequest<bool>
    {
        [JsonIgnore]
        public Guid WalletId { get; private set; }
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

        public void SetWalletId(Guid id)
        {
            WalletId = id;
        }
    }
}
