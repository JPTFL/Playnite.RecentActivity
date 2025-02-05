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
                    OnPropertyChanged(nameof(PlaytimeStatistics));
                    OnPropertyChanged(nameof(FormattedLastPlayed));
                    OnPropertyChanged(nameof(Game));
                    OnPropertyChanged(nameof(CoverImage));
                    OnPropertyChanged(nameof(FormattedSessionCount));
                    OnPropertyChanged(nameof(RelativePlaytimeRatio));
                }
            }
        }

        public string PlaytimeStatistics => string.Format(
            ResourceProvider.GetString("LOC_RecentActivity_PlaytimeStatistics"),
            TextConverter.SecondsToHoursText(Activity.Playtime, showHoursText: false),
            TextConverter.SecondsToHoursText(Game.Playtime),
            Activity.RecentPlayedRatio.ToString("P1", CultureInfo.InvariantCulture)
            );
        
        public string FormattedLastPlayed => string.Format(ResourceProvider.GetString("LOC_RecentActivity_LastPlayed"), Activity.LastPlayed.ToString("g"));
        public Game Game => Activity.Game;
        public string CoverImage => Activity.Game.CoverImage;
        public string FormattedSessionCount => string.Format(ResourceProvider.GetString("LOC_RecentActivity_SessionCount"), Activity.SessionCount);

        public string RelativePlaytimeRatio =>
            Activity.RelativePlaytimeRatio.ToString("P1", CultureInfo.InvariantCulture);
        
        public double RelativePlaytimeBarWidth =>  Activity.RelativePlaytimeRatio * 350;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}