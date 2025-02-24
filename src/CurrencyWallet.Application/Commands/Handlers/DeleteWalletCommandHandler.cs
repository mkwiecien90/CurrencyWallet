#region Usings

using CurrencyWallet.Application.Interfaces;
using CurrencyWallet.Domain.Exceptions;
using MediatR;

#endregion

namespace CurrencyWallet.Application.Commands.Handlers
{
    public class DeleteWalletCommandHandler : IRequestHandler<DeleteWalletCommand, bool>
    {
        private readonly IWalletRepository _walletRepository;

        public DeleteWalletCommandHandler(IWalletRepository walletRepository)
        {
            _walletRepository = walletRepository;
        }

        public async Task<bool> Handle(DeleteWalletCommand request, CancellationToken cancellationToken)
        {
            var wallet = await _walletRepository.GetByIdAsync(request.WalletId);

            if (wallet == null)
            {
                throw new NotFoundException($"Wallet with ID {request.WalletId} not found.");
            }

            await _walletRepository.DeleteAsync(wallet);

            return true;
        }
    }
}
