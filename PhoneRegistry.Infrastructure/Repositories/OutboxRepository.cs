using Microsoft.EntityFrameworkCore;
using PhoneRegistry.Domain.Entities;
using PhoneRegistry.Infrastructure.Data;

namespace PhoneRegistry.Infrastructure.Repositories;

public class OutboxRepository
{
    private readonly ContactDbContext _context;
    public OutboxRepository(ContactDbContext context) => _context = context;

    public Task AddAsync(OutboxMessage message, CancellationToken ct = default)
        => _context.OutboxMessages.AddAsync(message, ct).AsTask();

    public Task<List<OutboxMessage>> GetPendingBatchAsync(int take = 50, CancellationToken ct = default)
        => _context.OutboxMessages
            .Where(m => m.Status == OutboxStatus.Pending || m.Status == OutboxStatus.Failed)
            .OrderBy(m => m.OccurredAt)
            .Take(take)
            .ToListAsync(ct);
}


