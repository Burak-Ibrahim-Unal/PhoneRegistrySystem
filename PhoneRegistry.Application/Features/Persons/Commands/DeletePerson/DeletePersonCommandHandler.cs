using MediatR;
using Microsoft.Extensions.Logging;
using PhoneRegistry.Application.Common.Constants;
using PhoneRegistry.Domain.Repositories;

namespace PhoneRegistry.Application.Features.Persons.Commands.DeletePerson;

public class DeletePersonCommandHandler : IRequestHandler<DeletePersonCommand>
{
    private readonly IContactUnitOfWork _contactUnitOfWork;
    private readonly ILogger<DeletePersonCommandHandler> _logger;

    public DeletePersonCommandHandler(
        IContactUnitOfWork contactUnitOfWork,
        ILogger<DeletePersonCommandHandler> logger)
    {
        _contactUnitOfWork = contactUnitOfWork;
        _logger = logger;
    }

    public async Task Handle(DeletePersonCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(Messages.Person.Deleting, command.PersonId);

        var person = await _contactUnitOfWork.Persons.GetByIdAsync(command.PersonId, cancellationToken);
        if (person == null)
        {
            throw new ArgumentException(Messages.Person.NotFoundForDeletion.Replace("{PersonId}", command.PersonId.ToString()));
        }

        // Soft delete instead of hard delete
        person.SoftDelete();
        await _contactUnitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(Messages.Person.DeletedSuccessfully, command.PersonId);
    }
}
