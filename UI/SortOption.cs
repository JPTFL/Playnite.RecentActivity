using Playnite.SDK;

namespace RecentActivity.UI
{
    public enum SortOption
    {
        GameNameAscending,
        GameNameDescending,
        Playtime,
        LastPlayed,
        Sessions
    }

    public static class SortOptionExtensions
    {
        public static string GetDisplayName(this SortOption option)
        {
            // switch is not available in C# 7.0
            if (option == SortOption.GameNameAscending) return ResourceProvider.GetString("LOC_RecentActivity_SortOption_NameAscending");
            if (option == SortOption.GameNameDescending) return ResourceProvider.GetString("LOC_RecentActivity_SortOption_NameDescending");
            if (option == SortOption.Playtime) return ResourceProvider.GetString("LOC_RecentActivity_SortOption_Playtime");
            if (option == SortOption.LastPlayed) return ResourceProvider.GetString("LOC_RecentActivity_SortOption_LastPlayed");
            if (option == SortOption.Sessions)  return ResourceProvider.GetString("LOC_RecentActivity_SortOption_Sessions");

            // Fallback to the enum's string representation if no match is found
            return option.ToString();
        }
    }
}