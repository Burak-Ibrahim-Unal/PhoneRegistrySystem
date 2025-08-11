namespace PhoneRegistry.Domain.Entities;

public enum OutboxStatus
{
    Pending = 1,
    Processing = 2,
    Published = 3,
    Failed = 4
}

public class OutboxMessage : BaseEntity
{
    public string EventType { get; private set; } = string.Empty;
    public string Payload { get; private set; } = string.Empty;
    public DateTime OccurredAt { get; private set; } = DateTime.UtcNow;
    public DateTime? ProcessedAt { get; private set; }
    public OutboxStatus Status { get; private set; } = OutboxStatus.Pending;
    public int RetryCount { get; private set; } = 0;
    public string? LastError { get; private set; }

    protected OutboxMessage() { }

    public OutboxMessage(string eventType, string payload)
    {
        EventType = eventType;
        Payload = payload;
        OccurredAt = DateTime.UtcNow;
        Status = OutboxStatus.Pending;
    }

    public void MarkProcessing() => Status = OutboxStatus.Processing;
    public void MarkPublished()
    {
        Status = OutboxStatus.Published;
        ProcessedAt = DateTime.UtcNow;
    }
    public void MarkFailed(string error)
    {
        Status = OutboxStatus.Failed;
        RetryCount++;
        LastError = error;
    }
}


