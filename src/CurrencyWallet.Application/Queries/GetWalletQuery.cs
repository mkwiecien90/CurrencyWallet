#region Usings

using System.Text.Json.Serialization;
using CurrencyWallet.Application.DTOs;
using MediatR;

#endregion

namespace CurrencyWallet.Application.Queries
{
    public class GetWalletQuery : IRequest<WalletViewModel>
    {
        [JsonIgnore]
        public Guid WalletId { get; }

        public GetWalletQuery(Guid walletId)
        {
            WalletId = walletId;
        }
    }
}
