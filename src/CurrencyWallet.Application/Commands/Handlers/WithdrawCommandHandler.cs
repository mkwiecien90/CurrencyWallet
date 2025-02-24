#region Usings

using AutoMapper;
using CurrencyWallet.Application.Interfaces;
using CurrencyWallet.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

#endregion

namespace CurrencyWallet.Application.Commands.Handlers
{
    public class WithdrawCommandHandler : IRequestHandler<WithdrawCommand, bool>
    {
        private readonly IWalletRepository _walletRepository;
        private readonly ILogger<WithdrawCommandHandler> _logger;
        private readonly IMapper _mapper;

        public WithdrawCommandHandler(IWalletRepository walletRepository, ILogger<WithdrawCommandHandler> logger, IMapper mapper)
        {
            _walletRepository = walletRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<bool> Handle(WithdrawCommand request, CancellationToken cancellationToken)
        {
            var wallet = await _walletRepository.GetByIdAsync(request.WalletId);

            if (wallet == null)
            {
                throw new NotFoundException($"Wallet with ID {request.WalletId} not found.");
            }

            var canWithdraw = wallet.Withdraw(request.Currency, request.Amount);

            if (!canWithdraw)
            {
                throw new BadRequestException($"Can not withdraw from wallet {wallet.Name} value: {request.Amount} {request.Currency}.");
            }

            await _walletRepository.UpdateAsync(wallet);
            _logger.LogInformation($"Withdrawn from wallet {wallet.Name} value: {request.Amount} {request.Currency}.");

            return true;
        }
    }
}
