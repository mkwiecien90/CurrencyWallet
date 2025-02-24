#region Usings

using System.Text.Json.Serialization;
using MediatR;

#endregion

namespace CurrencyWallet.Application.Commands
{
    public class WithdrawCommand : IRequest<bool>
    {
        [JsonIgnore]
        public Guid WalletId { get; set; }
        public string Currency { get; set; }
        public decimal Amount { get; set; }

        public WithdrawCommand(Guid walletId, string currency, decimal amount)
        {
            WalletId = walletId;
            Currency = currency;
            Amount = amount;
        }
    }
}
