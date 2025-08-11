using Microsoft.EntityFrameworkCore.Storage;
using PhoneRegistry.Domain.Repositories;
using PhoneRegistry.Infrastructure.Data;

namespace PhoneRegistry.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ContactDbContext _contactContext;
    private readonly ReportDbContext _reportContext;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(ContactDbContext contactContext, ReportDbContext reportContext)
    {
        _contactContext = contactContext;
        _reportContext = reportContext;
        Persons = new PersonRepository(_contactContext);
        ContactInfos = new ContactInfoRepository(_contactContext);
        Reports = new ReportRepository(_reportContext);
    }

    public IPersonRepository Persons { get; }
    public IContactInfoRepository ContactInfos { get; }
    public IReportRepository Reports { get; }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var changes = 0;
        changes += await _contactContext.SaveChangesAsync(cancellationToken);
        changes += await _reportContext.SaveChangesAsync(cancellationToken);
        return changes;
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        // Not: İki ayrı DbContext aynı DB'yi işaret ediyor; tek transaction elde etmek için Connection paylaşımı gerekebilir.
        _transaction = await _contactContext.Database.BeginTransactionAsync(cancellationToken);
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
        _contactContext.Dispose();
        _reportContext.Dispose();
    }
}
