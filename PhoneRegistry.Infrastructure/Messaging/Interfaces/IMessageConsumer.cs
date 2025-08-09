namespace PhoneRegistry.Infrastructure.Messaging.Interfaces;

public interface IMessageConsumer<T> where T : class
{
    Task ConsumeAsync(T message, CancellationToken cancellationToken = default);
}
