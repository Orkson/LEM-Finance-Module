using Application.Abstractions;
using Domain.Abstraction;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly IApplicationDbContext _dbContext;

        public CompanyRepository(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddCompany(Company company, CancellationToken cancellationToken)
        {
            _dbContext.Companies.Add(company);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> CheckIfCompanyExists(string companyName, CancellationToken cancellationToken)
        {
            if(companyName == null)
            {
                return false;
            }
            return await _dbContext.Companies.AnyAsync(x => x.Name.ToLower() == companyName.ToLower(), cancellationToken);
        }

        public async Task<int> GetCompanyIdByItsName(string companyName, CancellationToken cancellationToken)
        {
            var company = await _dbContext.Companies.FirstAsync(x => x.Name.ToLower() == companyName.ToLower(), cancellationToken);
            return company.Id;
        }

        public async Task RemoveCompanyById(int companyId, CancellationToken cancellationToken)
        {
            var companyToBeRemoved = await _dbContext.Companies.FirstAsync(x => x.Id == companyId, cancellationToken);
            _dbContext.Companies.Remove(companyToBeRemoved);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
