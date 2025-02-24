#region Usings

using AutoMapper;
using CurrencyWallet.Application.Cache;
using MediatR;

#endregion

namespace CurrencyWallet.Application.Queries.Handlers
{
    public class GetAvaiableCurrenciesQueryHandler : IRequestHandler<GetAvaiableCurrenciesQuery, IEnumerable<string>>
    {
        private readonly ICurrencyCache _currencyCache;
        private readonly IMapper _mapper;

        public GetAvaiableCurrenciesQueryHandler(ICurrencyCache currencyCache, IMapper mapper)
        {
            _currencyCache = currencyCache;
            _mapper = mapper;
        }

        public async Task<IEnumerable<string>> Handle(GetAvaiableCurrenciesQuery request, CancellationToken cancellationToken)
        {
            return _currencyCache.GetAvailableCurrencies();
        }
    }
}
