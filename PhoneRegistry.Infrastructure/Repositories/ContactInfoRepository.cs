using Microsoft.EntityFrameworkCore;
using PhoneRegistry.Domain.Entities;
using PhoneRegistry.Domain.Repositories;
using PhoneRegistry.Domain.ValueObjects;
using PhoneRegistry.Infrastructure.Data;

namespace PhoneRegistry.Infrastructure.Repositories;

public class ContactInfoRepository : Repository<ContactInfo>, IContactInfoRepository
{
    public ContactInfoRepository(ContactDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<ContactInfo>> GetByPersonIdAsync(Guid personId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(ci => ci.PersonId == personId).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ContactInfo>> GetByTypeAsync(ContactType type, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(ci => ci.Type == type).ToListAsync(cancellationToken);
    }

    public async Task<Dictionary<string, int>> GetLocationStatisticsAsync(CancellationToken cancellationToken = default)
    {
        var locationContacts = await _dbSet
            .Where(ci => ci.Type == ContactType.Location)
            .GroupBy(ci => ci.Content)
            .Select(g => new { Location = g.Key, PersonCount = g.Count() })
            .ToListAsync(cancellationToken);

        return locationContacts.ToDictionary(x => x.Location, x => x.PersonCount);
    }
}
