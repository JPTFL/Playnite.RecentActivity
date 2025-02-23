using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Playnite.SDK;
using RecentActivity.Data;
using RecentActivity.Data.GameActivity;

namespace RecentActivity.Aggregator
{
    public class RecentActivityAggregator
    {
        private readonly GameActivityDataFetcher _dataFetcher;
        private readonly IPlayniteAPI _api;
        private IReadOnlyCollection<Activity> _activities;

        public RecentActivityAggregator(IPlayniteAPI api)
        {
            _dataFetcher = new GameActivityDataFetcher(api.Paths.ExtensionsDataPath);
            _api = api;
        }
        
        public async Task LoadActivityData()
        {
            _activities = await _dataFetcher.GetActivityForGames(_api.Database.Games);
        }
        
        public IReadOnlyCollection<RecentActivityData> GetRecentActivity(
            IPlayniteAPI api,
            DateTime startDate,
            DateTime endDate
        )
        {
            var recentActivity = new List<RecentActivityData>();
            var activitiesPlaytimeSum = _activities.Sum(a => a.Items.Sum(s => s.ElapsedSeconds));

            // grouped by game (Activity.Guid), filter all sessions (Activity.Items) that are within the date range (Session.DateSession)
            // is within the date range startDate <= Session.DateSession <= endDate and sum the ElapsedSeconds of all sessions as playtime
            // using Where, SelectMany, and Sum
            foreach (var activity in _activities)
            {
                ulong playtime = 0;
                ulong sessionCount = 0;
                var lastPlayed = DateTime.MinValue;
                foreach (var session in activity.Items)
                {
                    if (session.DateSession >= startDate && session.DateSession <= endDate)
                    {
                        sessionCount++;
                        playtime += (ulong)session.ElapsedSeconds;
                        if (session.DateSession > lastPlayed)
                        {
                            lastPlayed = session.DateSession;
                        }
                    }
                }

                if (playtime > 0)
                {
                    var game = api.Database.Games.FirstOrDefault(g =>
                        g.Id == activity.Id); // Assuming activity has GameId
                    if (game == null)
                    {
                        continue;
                    }

                    var relativePlaytimeRatio = ClampRelative((double)playtime / activitiesPlaytimeSum);

                    recentActivity.Add(new RecentActivityData
                    {
                        Game = game,
                        Playtime = playtime,
                        LastPlayed = lastPlayed,
                        SessionCount = sessionCount,
                        RecentPlayedRatio = ClampRelative((double)playtime / game.Playtime),
                        RelativePlaytimeRatio = relativePlaytimeRatio,
                    });
                }
            }

            var totalRelativePlaytime = recentActivity.Sum(x => x.RelativePlaytimeRatio);
            foreach (var activityData in recentActivity)
            {
                activityData.RelativePlaytimeRatio /= totalRelativePlaytime;
            }

            // sort by lastPlayed descending
            recentActivity.Sort((x, y) => y.LastPlayed.CompareTo(x.LastPlayed));
            return recentActivity;
        }

        private static double ClampRelative(double value, double min = 0, double max = 1)
        {
            return Math.Min(Math.Max(value, min), max);
        }
    }
}