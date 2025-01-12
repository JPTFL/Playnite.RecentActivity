using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Playnite.SDK;
using Playnite.SDK.Controls;
using Playnite.SDK.Models;
using RecentActivity.Converters;
using RecentActivity.Data;

namespace RecentActivity.UI
{

    public partial class MainView : PluginUserControl, INotifyPropertyChanged
    {
        private static readonly ILogger logger = LogManager.GetLogger();
        private ObservableCollection<RecentActivityEntry> _recentActivityList;

        public ObservableCollection<RecentActivityEntry> RecentActivityList
        {
            get => _recentActivityList;
            set
            {
                if (_recentActivityList != value)
                {
                    _recentActivityList = value;
                    OnPropertyChanged(nameof(RecentActivityList));
                }
            }
        }
        
        public string TotalPlaytimeText { get; set; }

        public MainView(IReadOnlyCollection<RecentActivityData> recentActivity)
        {
            InitializeComponent();
            DataContext = this;

            RecentActivityList = new ObservableCollection<RecentActivityEntry>();
            foreach (var activity in recentActivity)
            {
                RecentActivityList.Add(new RecentActivityEntry { Activity = activity });
            }
            var totalPlaytimeTextTemplate = ResourceProvider.GetString("LOC_RecentActivity_TotalPlaytime");
            var totalPlaytimeSeconds = CalculateTotalPlaytime(recentActivity);
            TotalPlaytimeText = string.Format(totalPlaytimeTextTemplate, TimeConverter.SecondsToHours(totalPlaytimeSeconds));
        }

        private int CalculateTotalPlaytime(IReadOnlyCollection<RecentActivityData> recentActivity)
        {
            var totalPlaytime = 0;
            foreach (var activity in recentActivity)
            {
                totalPlaytime += activity.playtime;
            }
            return totalPlaytime;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}