using PhoneRegistry.Domain.Entities;

namespace PhoneRegistry.Domain.Repositories;

public interface IPersonRepository : IRepository<Person>
{
    Task<Person?> GetByIdWithContactInfosAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Person>> GetAllWithContactInfosPagedAsync(int skip = 0, int take = 50, CancellationToken cancellationToken = default);
    Task<IEnumerable<Person>> GetAllWithContactInfosAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Person>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<int> GetTotalCountAsync(CancellationToken cancellationToken = default);
}
