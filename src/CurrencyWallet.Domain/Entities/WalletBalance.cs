using CurrencyWallet.Domain.ValueObjects;

namespace CurrencyWallet.Domain.Entities
{
    [Serializable]
    public class WalletBalance
    {
        public Money Money { get; set; }

        public WalletBalance(Money money) 
        {
            Money = money;
        }
    }

}
