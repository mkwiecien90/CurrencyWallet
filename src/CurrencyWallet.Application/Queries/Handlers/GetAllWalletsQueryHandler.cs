#region Usings

using AutoMapper;
using CurrencyWallet.Application.DTOs;
using CurrencyWallet.Application.Interfaces;
using MediatR;

#endregion

namespace CurrencyWallet.Application.Queries.Handlers
{
    public class GetAllWalletsQueryHandler : IRequestHandler<GetAllWalletsQuery, IEnumerable<WalletViewModel>>
    {
        private readonly IWalletRepository _walletRepository;
        private readonly IMapper _mapper;

        public GetAllWalletsQueryHandler(IWalletRepository walletRepository, IMapper mapper)
        {
            _walletRepository = walletRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<WalletViewModel>> Handle(GetAllWalletsQuery request, CancellationToken cancellationToken)
        {
            var wallets = await _walletRepository.GetAll();

            return _mapper.Map<List<WalletViewModel>>(wallets);
        }
    }
}
