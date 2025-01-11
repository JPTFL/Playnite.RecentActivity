using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Playnite.SDK;
using Playnite.SDK.Controls;
using Playnite.SDK.Models;
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

        public MainView(IReadOnlyCollection<RecentActivityData> recentActivity)
        {
            InitializeComponent();
            DataContext = this;

            RecentActivityList = new ObservableCollection<RecentActivityEntry>();
            foreach (var activity in recentActivity)
            {
                logger.Debug($"Adding JPT: {activity.Game.CoverImage}");
                RecentActivityList.Add(new RecentActivityEntry { Activity = activity });
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}