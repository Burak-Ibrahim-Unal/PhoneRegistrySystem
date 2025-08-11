using System.Text.Json;
using PhoneRegistry.Application.Common.Interfaces;
using PhoneRegistry.Domain.Entities;
using PhoneRegistry.Infrastructure.Data;

namespace PhoneRegistry.Infrastructure.Services;

public class OutboxWriter : IOutboxWriter
{
    private readonly ContactDbContext _context;
    public OutboxWriter(ContactDbContext context) => _context = context;

    public async Task EnqueueAsync(string eventType, object payload, CancellationToken cancellationToken = default)
    {
        var message = new OutboxMessage(eventType, JsonSerializer.Serialize(payload));
        await _context.OutboxMessages.AddAsync(message, cancellationToken);
    }
}


