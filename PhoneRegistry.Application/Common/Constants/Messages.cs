namespace PhoneRegistry.Application.Common.Constants;

public static class Messages
{
    // Person Operations
    public static class Person
    {
        public const string Creating = "Creating person: {FirstName} {LastName}";
        public const string CreatedSuccessfully = "Person created successfully";
        public const string GettingById = "Getting person by ID: {PersonId}";
        public const string NotFound = "Person not found: {PersonId}";
        public const string GettingAll = "Getting all persons with skip: {Skip}, take: {Take}";
        public const string RetrievedSuccessfully = "Retrieved persons successfully";
        public const string Deleting = "Deleting person: {PersonId}";
        public const string DeletedSuccessfully = "Person deleted successfully: {PersonId}";
        public const string NotFoundForDeletion = "Person with ID {PersonId} not found";
    }

    // Contact Info Operations
    public static class ContactInfo
    {
        public const string Adding = "Adding contact info for person: {PersonId}, type: {ContactType}";
        public const string AddedSuccessfully = "Contact info added successfully";
        public const string PersonNotFound = "Person with ID {PersonId} not found for contact info";
        public const string CreatedForPerson = "Contact info created for person: {PersonId}";
    }

    // Report Operations
    public static class Report
    {
        public const string Requesting = "Requesting new report";
        public const string RequestedSuccessfully = "Report requested successfully";
        public const string GettingAll = "Getting all reports";
        public const string GettingById = "Getting report by ID: {ReportId}";
        public const string Created = "Report created with ID: {ReportId}";
        public const string Processing = "Processing report: {ReportId}";
        public const string Completed = "Report completed: {ReportId}";
        public const string Failed = "Report failed: {ReportId}, Error: {Error}";
    }

    // Validation Messages
    public static class Validation
    {
        public const string ValidationFailed = "Validation failed for {CommandType}";
        public const string FirstNameRequired = "First name is required";
        public const string LastNameRequired = "Last name is required";
        public const string ContentRequired = "Content is required";
        public const string PersonIdRequired = "Person ID is required";
        public const string InvalidContactType = "Invalid contact type";
        public const string InvalidPersonId = "Invalid person ID format";
    }

    // General Operations
    public static class General
    {
        public const string OperationStarted = "Operation started: {Operation}";
        public const string OperationCompleted = "Operation completed: {Operation}";
        public const string OperationFailed = "Operation failed: {Operation}, Error: {Error}";
        public const string EntityNotFound = "Entity not found: {EntityType} with ID {EntityId}";
        public const string DatabaseSaveChanges = "Saving changes to database";
        public const string DatabaseSaveCompleted = "Database changes saved successfully";
    }

    // Cache Operations
    public static class Cache
    {
        public const string CacheHit = "Cache hit for key: {Key}";
        public const string CacheMiss = "Cache miss for key: {Key}";
        public const string CacheSet = "Setting cache for key: {Key}";
        public const string CacheRemove = "Removing cache for key: {Key}";
        public const string CacheError = "Cache operation error: {Error}";
    }

    // Message Queue Operations
    public static class MessageQueue
    {
        public const string PublishingMessage = "Publishing message to queue: {QueueName}";
        public const string MessagePublished = "Message published successfully to queue: {QueueName}";
        public const string ReceivingMessage = "Receiving message from queue: {QueueName}";
        public const string MessageReceived = "Message received from queue: {QueueName}";
        public const string MessageProcessingStarted = "Started processing message: {MessageId}";
        public const string MessageProcessingCompleted = "Completed processing message: {MessageId}";
        public const string MessageProcessingFailed = "Failed processing message: {MessageId}, Error: {Error}";
    }
}
