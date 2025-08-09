using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using PhoneRegistry.Application.Common.Constants;
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
        _logger.LogInformation(Messages.ContactInfo.Adding, command.PersonId, command.Type);

        var person = await _unitOfWork.Persons.GetByIdAsync(command.PersonId, cancellationToken);
        if (person == null)
        {
            throw new ArgumentException(Messages.ContactInfo.PersonNotFound.Replace("{PersonId}", command.PersonId.ToString()));
        }

        var contactInfo = person.AddContactInfo(command.Type, command.Content);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(Messages.ContactInfo.AddedSuccessfully);

        return _mapper.Map<ContactInfoDto>(contactInfo);
    }
}
