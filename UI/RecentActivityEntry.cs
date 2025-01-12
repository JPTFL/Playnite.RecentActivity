using System;
using System.ComponentModel;
using Playnite.SDK.Models;
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

        public string FormattedPlaytime => FormatPlaytime(Activity.playtime);
        public string FormattedLastPlayed => Activity.lastPlayed.ToString("g");
        public Game Game => Activity.Game;
        public string CoverImage => Activity.Game.CoverImage;

        private static string FormatPlaytime(int playtime)
        {
            var ts = new TimeSpan(0, 0, playtime);
            return $"Playtime: {ts:hh\\:mm}";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}