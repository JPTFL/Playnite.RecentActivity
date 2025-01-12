using System.Collections.Generic;
using RecentActivity.Data;

namespace RecentActivity.UI
{
    public interface IRecentActivityReceiver
    {
        void OnRecentActivityUpdated(IReadOnlyCollection<RecentActivityData> recentActivity);
    }
}