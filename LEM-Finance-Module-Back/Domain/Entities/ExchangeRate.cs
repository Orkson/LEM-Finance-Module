namespace Domain.Entities
{
    public class ExchangeRate
    {
        public Guid Id { get; set; }
        public string CurrencyCode { get; set; }
        public decimal RateToBase { get; set; }
        public DateTime Date { get; set; }
    }
}
