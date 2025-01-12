using System;

namespace RecentActivity.UI
{
    public interface IRecentActivitySetup
    {
        void RefreshData();
        void SetStartDate(DateTime startDate);
        void SetEndDate(DateTime endDate);
    }
}