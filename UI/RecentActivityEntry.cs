using System;
using System.ComponentModel;
using System.Globalization;
using Playnite.SDK;
using Playnite.SDK.Models;
using RecentActivity.Converters;
using RecentActivity.Data;

namespace RecentActivity.UI
{
    public class RecentActivityEntry : INotifyPropertyChanged
    {
        private RecentActivityData activity;
        public RecentActivityData Activity
        {
            get => activity;
            set
            {
                if (activity != value)
                {
                    activity = value;
                    OnPropertyChanged(nameof(Activity));
                    OnPropertyChanged(nameof(FormattedPlaytime));
                    OnPropertyChanged(nameof(FormattedLastPlayed));
                    OnPropertyChanged(nameof(Game));
                }
            }
        }

        public string FormattedPlaytime => TextConverter.SecondsToHoursText(Activity.Playtime);
        public string FormattedLastPlayed => Activity.LastPlayed.ToString("g");
        public Game Game => Activity.Game;
        public string CoverImage => Activity.Game.CoverImage;
        public string FormattedSessionCount => string.Format(ResourceProvider.GetString("LOC_RecentActivity_SessionCount"), Activity.SessionCount);
        public string RecentPlayedRatio => string.Format(ResourceProvider.GetString("LOC_RecentActivity_RecentPlayedRatio"), Activity.RecentPlayedRatio.ToString("P", CultureInfo.InvariantCulture));
        public string RelativePlaytimeRatio => string.Format(ResourceProvider.GetString("LOC_RecentActivity_RelativePlaytimeRatio"), Activity.RelativePlaytimeRatio.ToString("P", CultureInfo.InvariantCulture));

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}