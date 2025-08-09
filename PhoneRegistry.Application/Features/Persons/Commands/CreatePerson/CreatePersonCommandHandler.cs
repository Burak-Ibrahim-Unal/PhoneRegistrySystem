using FluentValidation;
using Microsoft.Extensions.Logging;
using PhoneRegistry.Application.Common.Constants;
using PhoneRegistry.Application.Common.Interfaces;
using PhoneRegistry.Domain.Entities;
using PhoneRegistry.Domain.Repositories;

namespace PhoneRegistry.Application.Features.Persons.Commands.CreatePerson;

public class CreatePersonCommandHandler : ICommandHandler<CreatePersonCommand, Person>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreatePersonCommand> _validator;
    private readonly ILogger<CreatePersonCommandHandler> _logger;

    public CreatePersonCommandHandler(
        IUnitOfWork unitOfWork,
        IValidator<CreatePersonCommand> validator,
        ILogger<CreatePersonCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Person> Handle(CreatePersonCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(Messages.Person.Creating, command.FirstName, command.LastName);

        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var person = new Person(command.FirstName, command.LastName, command.Company);
        
        await _unitOfWork.Persons.AddAsync(person, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(Messages.Person.CreatedSuccessfully);

        return person;
    }
}
