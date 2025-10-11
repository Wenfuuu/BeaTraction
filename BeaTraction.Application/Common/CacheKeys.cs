namespace BeaTraction.Application.Common;

public static class CacheKeys
{
    public const string AttractionStats = "attraction-stats";
    public const string UserAttractionsPrefix = "user-attractions";
    public const string SchedulesWithAttractions = "schedules-with-attractions";

    public static string GetUserAttractions(Guid userId)
    {
        return $"{UserAttractionsPrefix}:{userId}";
    }
}
