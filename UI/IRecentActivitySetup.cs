using System;

namespace RecentActivity.UI
{
    public interface IRecentActivitySetup
    {
        void RefreshData(bool reloadGamesFromApi = false);
        void SetStartDate(DateTime startDate);
        void SetEndDate(DateTime endDate);
        void SetSorting(SortOption sorting);
    }
}