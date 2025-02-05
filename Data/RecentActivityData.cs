using System;
using Playnite.SDK.Models;

namespace RecentActivity.Data
{
    public class RecentActivityData
    {
        public Game Game;
        public ulong Playtime;
        public DateTime LastPlayed;
        public ulong SessionCount;
        public double RecentPlayedRatio;
        public double RelativePlaytimeRatio;
    }
}