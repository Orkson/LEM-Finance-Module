using Domain.Entities;

namespace Domain.Abstraction
{
    public interface ICompanyRepository
    {
        Task AddCompany(Company company, CancellationToken cancellationToken = default);
        Task<bool> CheckIfCompanyExists(string companyName, CancellationToken cancellationToken = default);
        Task<int> GetCompanyIdByItsName(string companyName, CancellationToken cancellationToken = default);
        Task RemoveCompanyById(int companyId, CancellationToken cancellationToken);
    }
}
