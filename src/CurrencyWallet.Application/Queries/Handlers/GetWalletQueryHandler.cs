#region Usings

using AutoMapper;
using CurrencyWallet.Application.DTOs;
using CurrencyWallet.Application.Interfaces;
using CurrencyWallet.Domain.Exceptions;
using MediatR;

#endregion

namespace CurrencyWallet.Application.Queries.Handlers
{
    public class GetWalletQueryHandler : IRequestHandler<GetWalletQuery, WalletViewModel>
    {
        private readonly IWalletRepository _walletRepository;
        private readonly IMapper _mapper;

        public GetWalletQueryHandler(IWalletRepository walletRepository, IMapper mapper)
        {
            _walletRepository = walletRepository;
            _mapper = mapper;
        }

        public async Task<WalletViewModel> Handle(GetWalletQuery request, CancellationToken cancellationToken)
        {
            var wallet = await _walletRepository.GetByIdAsync(request.WalletId);

            if (wallet == null)
            {
                throw new NotFoundException($"Wallet with ID {request.WalletId} not found.");
            }

            return _mapper.Map<WalletViewModel>(wallet);
        }
    }
}
