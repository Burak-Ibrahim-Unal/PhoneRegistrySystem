using PhoneRegistry.Domain.Repositories;
using PhoneRegistry.Infrastructure.Data;

namespace PhoneRegistry.Infrastructure.Repositories;

public class ReportUnitOfWork : IReportUnitOfWork
{
    private readonly ReportDbContext _context;

    public ReportUnitOfWork(ReportDbContext context)
    {
        _context = context;
        Reports = new ReportRepository(_context);
    }

    public IReportRepository Reports { get; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);

    public void Dispose() => _context.Dispose();
}


