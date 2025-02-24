#region Usings

using MediatR;

#endregion

namespace CurrencyWallet.Application.Commands
{
    public class DeleteWalletCommand : IRequest<bool>
    {
        public Guid WalletId { get; }

        public DeleteWalletCommand(Guid walletId)
        {
            WalletId = walletId;
        }
    }
}
