using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Playnite.SDK;
using Playnite.SDK.Models;
using RecentActivity.Data;

namespace RecentActivity.Aggregator
{
    public class RecentActivityAggregator
    {
        private static readonly ILogger _logger = LogManager.GetLogger("RecentActivityAggregator");
        
        public static async Task<IReadOnlyCollection<RecentActivityData>> GetRecentActivity(
            IPlayniteAPI api,
            DateTime startDate, 
            DateTime endDate
            )
        {
            var dataFetcher = new GameActivityDataFetcher(api.Paths.ExtensionsDataPath);
            var activities = await dataFetcher.GetActivityForGames(api.Database.Games);
            var recentActivity = new List<RecentActivityData>();
            // grouped by game (Activity.Guid), filter all sessions (Activity.Items) that are within the date range (Session.DateSession)
            // is within the date range startDate <= Session.DateSession <= endDate and sum the ElapsedSeconds of all sessions as playtime
            // using Where, SelectMany, and Sum
            foreach (var activity in activities)
            {
                var playtime = 0;
                var sessionCount = 0;
                var lastPlayed = DateTime.MinValue;
                foreach (var session in activity.Items)
                {
                    if (session.DateSession >= startDate && session.DateSession <= endDate)
                    {
                        sessionCount++;
                        playtime += session.ElapsedSeconds;
                        if (session.DateSession > lastPlayed)
                        {
                            lastPlayed = session.DateSession;
                        }
                    }
                }
                if (playtime > 0)
                {
                    var game = api.Database.Games.FirstOrDefault(g => g.Id == activity.Id); // Assuming activity has GameId
                    recentActivity.Add(new RecentActivityData
                    {
                        Game = game,
                        Playtime = playtime,
                        LastPlayed = lastPlayed,
                        SessionCount = sessionCount
                    });
                }
            }
            _logger.Info($"Found {recentActivity.Count} recent activities");
            
            // sort by lastPlayed descending
            recentActivity.Sort((x, y) => y.LastPlayed.CompareTo(x.LastPlayed));
            return recentActivity;
        }
    }
}