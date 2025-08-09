using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using PhoneRegistry.Application.Common.DTOs;
using PhoneRegistry.Domain.Repositories;

namespace PhoneRegistry.Application.Features.ContactInfos.Commands.AddContactInfo;

public class AddContactInfoCommandHandler : IRequestHandler<AddContactInfoCommand, ContactInfoDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<AddContactInfoCommandHandler> _logger;

    public AddContactInfoCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<AddContactInfoCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ContactInfoDto> Handle(AddContactInfoCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Adding contact info for person: {PersonId}", command.PersonId);

        var person = await _unitOfWork.Persons.GetByIdAsync(command.PersonId, cancellationToken);
        if (person == null)
        {
            throw new ArgumentException($"Person with ID {command.PersonId} not found");
        }

        var contactInfo = person.AddContactInfo(command.Type, command.Content);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Contact info added successfully: {ContactInfoId}", contactInfo.Id);

        return _mapper.Map<ContactInfoDto>(contactInfo);
    }
}
