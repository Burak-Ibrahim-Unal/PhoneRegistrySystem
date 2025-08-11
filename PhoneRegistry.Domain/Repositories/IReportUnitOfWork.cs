namespace PhoneRegistry.Domain.Repositories;

public interface IReportUnitOfWork : IDisposable
{
    IReportRepository Reports { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}


