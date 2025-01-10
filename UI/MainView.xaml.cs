using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Playnite.SDK.Controls;
using System.ComponentModel;
using System.Text;
using RecentActivity.Data;

namespace RecentActivity.UI
{
    public partial class MainView : PluginUserControl, INotifyPropertyChanged
    {
        private string _recentActivity;

        public string RecentActivity
        {
            get => _recentActivity;
            set
            {
                if (_recentActivity != value)
                {
                    _recentActivity = value;
                    OnPropertyChanged(nameof(RecentActivity));
                }
            }
        }

        public MainView(IReadOnlyCollection<RecentActivityData> recentActivity)
        {
            InitializeComponent();
            DataContext = this; 
            
            RecentActivity = FormatRecentActivity(recentActivity);
        }

        private static string FormatRecentActivity(IReadOnlyCollection<RecentActivityData> recentActivity)
        {
            var sb = new StringBuilder();
            foreach (var activity in recentActivity)
            {
                sb.AppendLine($"{activity.Game.Name} - {FormatPlaytime(activity.playtime)} - {activity.lastPlayed}");
            }
            return sb.ToString();
        }
        
        private static string FormatPlaytime(int playtime)
        {
            var ts = new TimeSpan(0, 0, playtime);
            return ts.ToString();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}