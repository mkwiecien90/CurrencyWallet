namespace CurrencyWallet.Domain.Entities
{
    [Serializable]
    public class CurrencyRate
    {
        public int Id { get; set; }
        public string Currency { get; set; }
        public string Code { get; set; }
        public decimal Mid { get; set; }
    }
}
