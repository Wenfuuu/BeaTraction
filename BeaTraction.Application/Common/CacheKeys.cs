namespace BeaTraction.Application.Common;

public static class CacheKeys
{
    public const string AttractionStats = "attraction-stats";
    public const string UserAttractionsPrefix = "user-attractions";
    public const string SchedulesWithAttractions = "schedules-with-attractions";
    private const string RegistrationCountPrefix = "capacity";
    
    public static string GetUserAttractions(Guid userId)
    {
        return $"{UserAttractionsPrefix}:{userId}";
    }

    public static string GetUserAttractions(string userId)
    {
        return $"{UserAttractionsPrefix}:{userId}";
    }

    public static string GetRegistrationCount(Guid scheduleAttractionId)
    {
        return $"{RegistrationCountPrefix}:{scheduleAttractionId}";
    }
}
