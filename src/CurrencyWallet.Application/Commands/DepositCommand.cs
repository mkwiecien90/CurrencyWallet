#region Usings

using System.Text.Json.Serialization;
using CurrencyWallet.Application.DTOs;
using MediatR;

#endregion

namespace CurrencyWallet.Application.Commands
{
    public class DepositCommand : IRequest<bool>
    {
        [JsonIgnore]
        public Guid WalletId { get; set; }
        public string Currency { get; set; }
        public decimal Amount { get; set; }

        public DepositCommand(Guid walletId, string currency, decimal amount)
        {
            WalletId = walletId;
            Currency = currency;
            Amount = amount;
        }
    }
}
