using PhoneRegistry.Domain.Entities;
using PhoneRegistry.Domain.ValueObjects;

namespace PhoneRegistry.Domain.Repositories;

public interface IContactInfoRepository : IRepository<ContactInfo>
{
    Task<IEnumerable<ContactInfo>> GetByPersonIdAsync(Guid personId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ContactInfo>> GetByTypeAsync(ContactType type, CancellationToken cancellationToken = default);
    Task<Dictionary<string, int>> GetLocationStatisticsAsync(CancellationToken cancellationToken = default);
}
