using Microsoft.EntityFrameworkCore.Storage;
using PhoneRegistry.Domain.Repositories;
using PhoneRegistry.Infrastructure.Data;

namespace PhoneRegistry.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly PhoneRegistryDbContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(PhoneRegistryDbContext context)
    {
        _context = context;
        Persons = new PersonRepository(_context);
        ContactInfos = new ContactInfoRepository(_context);
        Reports = new ReportRepository(_context);
    }

    public IPersonRepository Persons { get; }
    public IContactInfoRepository ContactInfos { get; }
    public IReportRepository Reports { get; }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
