using Microsoft.EntityFrameworkCore.Storage;

namespace Domain.Abstraction
{
    public interface IUnitOfWork
    {
        Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task CommitAsync(CancellationToken cancellationToken = default);
        void Rollback();
    }
}
