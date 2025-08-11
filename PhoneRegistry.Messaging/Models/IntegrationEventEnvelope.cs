using System.Text.Json;

namespace PhoneRegistry.Messaging.Models;

public class IntegrationEventEnvelope
{
    public string EventType { get; set; } = string.Empty;
    public JsonElement Payload { get; set; }
}


