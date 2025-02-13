using Application.Abstractions;
using Domain.Abstraction;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly IApplicationDbContext _dbContext;
    private IDbContextTransaction _transaction;

    public UnitOfWork(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        return _transaction;
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync(cancellationToken);
            _transaction.Dispose();
            _transaction = null;
        }
    }

    public void Rollback()
    {
        if (_transaction != null)
        {
            _transaction.Rollback();
            _transaction.Dispose();
            _transaction = null;
        }
    }
}
