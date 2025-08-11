using PhoneRegistry.Domain.Repositories;
using PhoneRegistry.Infrastructure.Data;

namespace PhoneRegistry.Infrastructure.Repositories;

public class ContactUnitOfWork : IContactUnitOfWork
{
    private readonly ContactDbContext _context;

    public ContactUnitOfWork(ContactDbContext context)
    {
        _context = context;
        Persons = new PersonRepository(_context);
        ContactInfos = new ContactInfoRepository(_context);
    }

    public IPersonRepository Persons { get; }
    public IContactInfoRepository ContactInfos { get; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);

    public void Dispose() => _context.Dispose();
}


