#region Usings

using MediatR;

#endregion

namespace CurrencyWallet.Application.Queries
{
    public class GetAvaiableCurrenciesQuery : IRequest<IEnumerable<string>>
    {
    }
}