#region Usings

using System.Text.Json.Serialization;
using MediatR;

#endregion

namespace CurrencyWallet.Application.Commands
{
    public class WithdrawCommand : IRequest<bool>
    {
        [JsonIgnore]
        public Guid WalletId { get; private set; }
        public string Currency { get; }
        public decimal Amount { get; }

        public WithdrawCommand(Guid walletId, string currency, decimal amount)
        {
            WalletId = walletId;
            Currency = currency;
            Amount = amount;
        }

        public void SetWalletId(Guid id)
        {
            WalletId = id;
        }
    }
}
