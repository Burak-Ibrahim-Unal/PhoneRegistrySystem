namespace PhoneRegistry.Application.Common.Messaging;

public record PersonUpserted(Guid PersonId, string FirstName, string LastName, string? Company);
public record ContactInfoUpserted(Guid PersonId, Guid ContactInfoId, int Type, string Content, string? CityName);
public record ContactInfoDeleted(Guid PersonId, Guid ContactInfoId);


