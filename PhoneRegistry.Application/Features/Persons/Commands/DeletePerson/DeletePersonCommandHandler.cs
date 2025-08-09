using MediatR;
using Microsoft.Extensions.Logging;
using PhoneRegistry.Domain.Repositories;

namespace PhoneRegistry.Application.Features.Persons.Commands.DeletePerson;

public class DeletePersonCommandHandler : IRequestHandler<DeletePersonCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeletePersonCommandHandler> _logger;

    public DeletePersonCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<DeletePersonCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(DeletePersonCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting person with ID: {PersonId}", command.PersonId);

        var person = await _unitOfWork.Persons.GetByIdAsync(command.PersonId, cancellationToken);
        if (person == null)
        {
            throw new ArgumentException($"Person with ID {command.PersonId} not found");
        }

        await _unitOfWork.Persons.DeleteAsync(person, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Person deleted successfully: {PersonId}", command.PersonId);
    }
}
