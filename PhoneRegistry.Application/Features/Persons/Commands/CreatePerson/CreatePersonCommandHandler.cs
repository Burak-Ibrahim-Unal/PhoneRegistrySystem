using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using PhoneRegistry.Application.Common.DTOs;
using PhoneRegistry.Domain.Entities;
using PhoneRegistry.Domain.Repositories;

namespace PhoneRegistry.Application.Features.Persons.Commands.CreatePerson;

public class CreatePersonCommandHandler : IRequestHandler<CreatePersonCommand, PersonDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<CreatePersonCommand> _validator;
    private readonly ILogger<CreatePersonCommandHandler> _logger;

    public CreatePersonCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<CreatePersonCommand> validator,
        ILogger<CreatePersonCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validator = validator;
        _logger = logger;
    }

    public async Task<PersonDto> Handle(CreatePersonCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating person: {FirstName} {LastName}", command.FirstName, command.LastName);

        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var person = new Person(command.FirstName, command.LastName, command.Company);
        
        await _unitOfWork.Persons.AddAsync(person, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Person created successfully with ID: {PersonId}", person.Id);

        return _mapper.Map<PersonDto>(person);
    }
}
