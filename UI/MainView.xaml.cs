using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Playnite.SDK;
using Playnite.SDK.Controls;
using RecentActivity.Converters;
using RecentActivity.Data;

namespace RecentActivity.UI
{
    public partial class MainView : PluginUserControl, INotifyPropertyChanged, IRecentActivityReceiver
    {
        private static readonly ILogger logger = LogManager.GetLogger();
        private ObservableCollection<RecentActivityEntry> _recentActivityList;
        private DateTime _startDate;
        private DateTime _endDate;
        private string _totalPlaytimeText;
        private IRecentActivitySetup _recentActivitySetup;

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

        public string StartDateText { get; set; }
        public string EndDateText { get; set; }
        
        public string LastYearText { get; set; } = ResourceProvider.GetString("LOC_RecentActivity_LastYear");
        public string LastMonthText { get; set; } = ResourceProvider.GetString("LOC_RecentActivity_LastMonth");
        public string LastTwoWeeksText { get; set; } = ResourceProvider.GetString("LOC_RecentActivity_LastTwoWeeks");
        public string LastWeekText { get; set; } = ResourceProvider.GetString("LOC_RecentActivity_LastWeek");
        
        public RelayCommand LastYearCommand { get; set; }
        public RelayCommand LastMonthCommand { get; set; }
        public RelayCommand LastTwoWeeksCommand { get; set; }
        public RelayCommand LastWeekCommand { get; set; }

        public string TotalPlaytimeText
        {
            get => _totalPlaytimeText;
            set
            {
                if (_totalPlaytimeText != value)
                {
                    _totalPlaytimeText = value;
                    OnPropertyChanged(nameof(TotalPlaytimeText));
                }
            }
        }

        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                if (_startDate != value)
                {
                    _startDate = value;
                    _recentActivitySetup.SetStartDate(value);
                    OnPropertyChanged(nameof(StartDate));
                }
            }
        }
        public DateTime EndDate 
        {
            get => _endDate;
            set
            {
                if (_endDate != value)
                {
                    _endDate = value;
                    _recentActivitySetup.SetEndDate(value);
                    OnPropertyChanged(nameof(EndDate));
                }
            }
        }

        public MainView(
            DateTime startDate,
            DateTime endDate,
            IRecentActivitySetup recentActivitySetup
            )
        {
            InitializeComponent();
            DataContext = this;
            _recentActivitySetup = recentActivitySetup;
            
            StartDate = startDate;
            EndDate = endDate;
            
            StartDateText = ResourceProvider.GetString("LOC_RecentActivity_StartDate");
            EndDateText = ResourceProvider.GetString("LOC_RecentActivity_EndDate");
            
            // command implementations
            LastYearCommand = new RelayCommand(() => SetDateRange(DateTime.Now.AddYears(-1), DateTime.Now));
            LastMonthCommand = new RelayCommand(() => SetDateRange(DateTime.Now.AddMonths(-1), DateTime.Now));
            LastTwoWeeksCommand = new RelayCommand(() => SetDateRange(DateTime.Now.AddDays(-14), DateTime.Now));
            LastWeekCommand = new RelayCommand(() => SetDateRange(DateTime.Now.AddDays(-7), DateTime.Now));

        }

        private void SetDateRange(DateTime start, DateTime end)
        {
            StartDate = start;
            EndDate = end;
        }

        private int CalculateTotalPlaytime(IReadOnlyCollection<RecentActivityData> recentActivity)
        {
            var totalPlaytime = 0;
            foreach (var activity in recentActivity)
            {
                totalPlaytime += activity.Playtime;
            }

            return totalPlaytime;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void OnRecentActivityUpdated(IReadOnlyCollection<RecentActivityData> recentActivity)
        {
            RecentActivityList = new ObservableCollection<RecentActivityEntry>();
            foreach (var activity in recentActivity)
            {
                RecentActivityList.Add(new RecentActivityEntry { Activity = activity });
            }            

            var totalPlaytimeTextTemplate = ResourceProvider.GetString("LOC_RecentActivity_TotalPlaytime");
            var totalPlaytimeSeconds = CalculateTotalPlaytime(recentActivity);
            TotalPlaytimeText =
                string.Format(totalPlaytimeTextTemplate, TextConverter.SecondsToHoursText(totalPlaytimeSeconds));
        }
    }
}