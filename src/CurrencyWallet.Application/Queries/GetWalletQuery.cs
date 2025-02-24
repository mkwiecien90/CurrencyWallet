#region Usings

using CurrencyWallet.Application.DTOs;
using MediatR;

#endregion

namespace CurrencyWallet.Application.Queries
{
    public class GetWalletQuery : IRequest<WalletViewModel>
    {
        public Guid WalletId { get; }

        public GetWalletQuery(Guid walletId)
        {
            WalletId = walletId;
        }
    }
}
