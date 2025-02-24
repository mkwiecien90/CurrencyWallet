#region Usings

using CurrencyWallet.Domain.Entities;

#endregion

namespace CurrencyWallet.Application.DTOs
{
    public class ExchangeRatesTable
    {
        public string? Table { get; set; }
        public string? No { get; set; }
        public string? EffectiveDate { get; set; }
        public List<CurrencyRate>? Rates { get; set; }
    }
}
