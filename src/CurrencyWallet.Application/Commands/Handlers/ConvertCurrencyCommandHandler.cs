#region Usings

using AutoMapper;
using CurrencyWallet.Application.Interfaces;
using CurrencyWallet.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

#endregion

namespace CurrencyWallet.Application.Commands.Handlers
{
    public class ConvertCurrencyCommandHandler : IRequestHandler<ConvertCurrencyCommand, bool>
    {
        private readonly IWalletRepository _walletRepository;
        private readonly ICurrencyRateRepository _currencyRateRepository;
        private readonly ILogger<ConvertCurrencyCommandHandler> _logger;
        private readonly IMapper _mapper;

        public ConvertCurrencyCommandHandler(IWalletRepository walletRepository, ICurrencyRateRepository currencyRateRepository, ILogger<ConvertCurrencyCommandHandler> logger, IMapper mapper)
        {
            _walletRepository = walletRepository;
            _currencyRateRepository = currencyRateRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<bool> Handle(ConvertCurrencyCommand request, CancellationToken cancellationToken)
        {
            var wallet = await _walletRepository.GetByIdAsync(request.WalletId);

            if (wallet == null)
            {
                throw new NotFoundException($"Wallet with ID {request.WalletId} not found.");
            }

            var currencyRates = await _currencyRateRepository.GetAllAsync();

            if (currencyRates is null || !currencyRates.Any())
            {
                throw new NotFoundException($"Can not find currency rates.");
            }

            wallet.Convert(request.Amount, request.FromCurrency, request.ToCurrency, currencyRates);

            await _walletRepository.UpdateAsync(wallet);
            _logger.LogInformation($"Converted on wallet {wallet.Name}: {request.Amount} {request.FromCurrency} to currency {request.ToCurrency}.");

            return true;
        }
    }
}
