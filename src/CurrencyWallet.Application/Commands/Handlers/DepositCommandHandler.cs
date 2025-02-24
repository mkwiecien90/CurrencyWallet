#region Usings

using AutoMapper;
using CurrencyWallet.Application.Interfaces;
using CurrencyWallet.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

#endregion

namespace CurrencyWallet.Application.Commands.Handlers
{
    public class DepositCommandHandler : IRequestHandler<DepositCommand, bool>
    {
        private readonly IWalletRepository _walletRepository;
        private readonly ILogger<DepositCommandHandler> _logger;
        private readonly IMapper _mapper;

        public DepositCommandHandler(IWalletRepository walletRepository, ILogger<DepositCommandHandler> logger, IMapper mapper)
        {
            _walletRepository = walletRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<bool> Handle(DepositCommand request, CancellationToken cancellationToken)
        {
            var wallet = await _walletRepository.GetByIdAsync(request.WalletId);

            if (wallet == null)
            {
                throw new NotFoundException($"Wallet with ID {request.WalletId} not found.");
            }

            wallet.Deposit(request.Currency, request.Amount);

            await _walletRepository.UpdateAsync(wallet);
            _logger.LogInformation($"Deposit to wallet {wallet.Name} value: {request.Amount} {request.Currency}.");

            return true;
        }
    }
}
