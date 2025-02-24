#region Usings

using CurrencyWallet.Application.DTOs;
using MediatR;

#endregion

namespace CurrencyWallet.Application.Queries
{
    public class GetAllWalletsQuery : IRequest<IEnumerable<WalletViewModel>>
    {
    }
}
