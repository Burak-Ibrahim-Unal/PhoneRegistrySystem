namespace PhoneRegistry.Domain.Repositories;

public interface IContactUnitOfWork : IDisposable
{
    IPersonRepository Persons { get; }
    IContactInfoRepository ContactInfos { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}


