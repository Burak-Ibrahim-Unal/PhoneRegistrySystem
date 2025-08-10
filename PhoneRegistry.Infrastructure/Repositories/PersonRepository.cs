using Microsoft.EntityFrameworkCore;
using PhoneRegistry.Domain.Entities;
using PhoneRegistry.Domain.Repositories;
using PhoneRegistry.Infrastructure.Data;

namespace PhoneRegistry.Infrastructure.Repositories;

public class PersonRepository : Repository<Person>, IPersonRepository
{
    public PersonRepository(PhoneRegistryDbContext context) : base(context)
    {
    }

    public async Task<Person?> GetByIdWithContactInfosAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => !p.IsDeleted)
            .Include(p => p.ContactInfos.Where(c => !c.IsDeleted))
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Person>> GetAllWithContactInfosPagedAsync(int skip = 0, int take = 50, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => !p.IsDeleted)
            .Include(p => p.ContactInfos.Where(c => !c.IsDeleted))
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Person>> GetAllWithContactInfosAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => !p.IsDeleted)
            .Include(p => p.ContactInfos.Where(c => !c.IsDeleted))
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Person>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => !p.IsDeleted && (p.FirstName.Contains(searchTerm) || p.LastName.Contains(searchTerm)))
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetTotalCountAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(p => !p.IsDeleted).CountAsync(cancellationToken);
    }
}
