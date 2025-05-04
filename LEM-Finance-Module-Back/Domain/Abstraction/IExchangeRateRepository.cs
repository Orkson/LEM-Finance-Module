using Domain.Entities;

namespace Domain.Abstraction
{
    public interface IExchangeRateRepository
    {
        Task<ExchangeRate> GetLatestRateAsync(string currencyCode);
        Task<List<ExchangeRate>> GetRatesAsync();
        Task<ExchangeRate> AddRateAsync(ExchangeRate rate);
    }
}
