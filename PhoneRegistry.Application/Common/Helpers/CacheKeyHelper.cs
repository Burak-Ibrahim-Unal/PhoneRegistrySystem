namespace PhoneRegistry.Application.Common.Helpers;

public static class CacheKeyHelper
{
    private const string Separator = ":";
    
    // Person cache keys
    public static string GetPersonByIdKey(Guid personId) 
        => $"person{Separator}{personId}";
    
    public static string GetAllPersonsKey(int skip, int take) 
        => $"persons{Separator}list{Separator}{skip}{Separator}{take}";
    
    public static string GetPersonsListPattern() 
        => $"persons{Separator}list{Separator}*";
    
    // Report cache keys
    public static string GetReportByIdKey(Guid reportId) 
        => $"report{Separator}{reportId}";
    
    public static string GetAllReportsKey() 
        => $"reports{Separator}all";
    
    // City cache keys
    public static string GetAllCitiesKey() 
        => $"cities{Separator}all";
    
    // Generic pattern helpers
    public static string GetEntityKey<T>(Guid id) where T : class
        => $"{typeof(T).Name.ToLowerInvariant()}{Separator}{id}";
    
    public static string GetListKey<T>(params object[] parameters) where T : class
    {
        var entityName = typeof(T).Name.ToLowerInvariant();
        var paramString = string.Join(Separator, parameters);
        return $"{entityName}s{Separator}list{Separator}{paramString}";
    }
}
