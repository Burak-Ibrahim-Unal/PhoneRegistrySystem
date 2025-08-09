using PhoneRegistry.Domain.Entities;

namespace PhoneRegistry.Services.Interfaces;

public interface IPersonService
{
    Task<Person> CreatePersonAsync(string firstName, string lastName, string? company = null, CancellationToken cancellationToken = default);
    Task<Person?> GetPersonByIdAsync(Guid personId, CancellationToken cancellationToken = default);
    Task<List<Person>> GetAllPersonsAsync(int skip = 0, int take = 50, CancellationToken cancellationToken = default);
    Task DeletePersonAsync(Guid personId, CancellationToken cancellationToken = default);
    Task<ContactInfo> AddContactInfoAsync(Guid personId, int contactType, string content, CancellationToken cancellationToken = default);
    Task RemoveContactInfoAsync(Guid personId, Guid contactInfoId, CancellationToken cancellationToken = default);
}
