public static class HeroAvailability
{
    public static bool IsAvailable(bool permanentlyUnlocked, bool developmentUnlock, bool isEditor, bool isDevelopmentBuild)
        => permanentlyUnlocked || (developmentUnlock && (isEditor || isDevelopmentBuild));
}
