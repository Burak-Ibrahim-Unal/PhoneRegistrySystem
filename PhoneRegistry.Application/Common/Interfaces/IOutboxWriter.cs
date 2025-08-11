namespace PhoneRegistry.Application.Common.Interfaces;

public interface IOutboxWriter
{
    Task EnqueueAsync(string eventType, object payload, CancellationToken cancellationToken = default);
}


