using Application.Abstractions;
using Domain.Abstraction;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ExchangeRateRepository : IExchangeRateRepository
    {
        private readonly IApplicationDbContext _context;

        public ExchangeRateRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ExchangeRate> GetLatestRateAsync(string currencyCode)
        {
            return await _context.ExchangeRates
                .Where(r => r.CurrencyCode == currencyCode)
                .OrderByDescending(r => r.Date)
                .FirstOrDefaultAsync();
        }

        public async Task<List<ExchangeRate>> GetRatesAsync()
        {
            return await _context.ExchangeRates.ToListAsync();
        }

        public async Task<ExchangeRate> AddRateAsync(ExchangeRate rate)
        {
            _context.ExchangeRates.Add(rate);
            await _context.SaveChangesAsync();
            return rate;
        }
    }
}
