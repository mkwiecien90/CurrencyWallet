#region Usings

using CurrencyWallet.Domain.Entities;

#endregion

namespace CurrencyWallet.Application.DTOs
{
    public class WalletViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public IList<WalletBalance> Balances { get; set; }
    }
}
