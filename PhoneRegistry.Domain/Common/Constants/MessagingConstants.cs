namespace PhoneRegistry.Domain.Common.Constants;

public static class MessagingConstants
{
    public static class Queues
    {
        public const string ReportProcessing = "report-processing-queue";
        public const string ContactEvents = "contact-events";
    }

    public static class EventTypes
    {
        public const string PersonUpserted = "PersonUpserted";
        public const string ContactInfoUpserted = "ContactInfoUpserted";
        public const string ContactInfoDeleted = "ContactInfoDeleted";
    }

    public static class Configuration
    {
        public const int DefaultRetryCount = 3;
        public const int DefaultTimeoutSeconds = 30;
        public const int CircuitBreakerThreshold = 5;
        public const int CircuitBreakerDurationSeconds = 30;
    }
}
