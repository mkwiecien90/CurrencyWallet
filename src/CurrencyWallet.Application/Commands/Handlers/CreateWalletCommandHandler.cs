#region Usings

using CurrencyWallet.Application.Interfaces;
using CurrencyWallet.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

#endregion

namespace CurrencyWallet.Application.Commands.Handlers
{
    public class CreateWalletCommandHandler : IRequestHandler<CreateWalletCommand, Guid>
    {
        private readonly IWalletRepository _walletRepository;
        private readonly ILogger<CreateWalletCommandHandler> _logger;

        public CreateWalletCommandHandler(IWalletRepository walletRepository, ILogger<CreateWalletCommandHandler> logger)
        {
            _walletRepository = walletRepository;
            _logger = logger;
        }

        public async Task<Guid> Handle(CreateWalletCommand request, CancellationToken cancellationToken)
        {
            var wallet = new Wallet
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Balances = new List<WalletBalance>()
            };

            await _walletRepository.AddAsync(wallet);
            _logger.LogInformation($"Wallet {request.Name} created.");
            return wallet.Id;
        }
    }
}
