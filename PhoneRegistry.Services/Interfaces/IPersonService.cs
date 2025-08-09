using PhoneRegistry.Application.Common.DTOs;

namespace PhoneRegistry.Services.Interfaces;

public interface IPersonService
{
    Task<PersonDto> CreatePersonAsync(string firstName, string lastName, string? company = null, CancellationToken cancellationToken = default);
    Task<PersonDto?> GetPersonByIdAsync(Guid personId, CancellationToken cancellationToken = default);
    Task<List<PersonSummaryDto>> GetAllPersonsAsync(int skip = 0, int take = 50, CancellationToken cancellationToken = default);
    Task DeletePersonAsync(Guid personId, CancellationToken cancellationToken = default);
    Task<ContactInfoDto> AddContactInfoAsync(Guid personId, int contactType, string content, CancellationToken cancellationToken = default);
    Task RemoveContactInfoAsync(Guid personId, Guid contactInfoId, CancellationToken cancellationToken = default);
}
