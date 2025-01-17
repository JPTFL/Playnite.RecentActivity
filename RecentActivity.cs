using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using Playnite.SDK;
using Playnite.SDK.Events;
using Playnite.SDK.Plugins;
using RecentActivity.Aggregator;
using RecentActivity.Data;
using RecentActivity.UI;

namespace RecentActivity
{
    public class RecentActivity : GenericPlugin, IRecentActivitySetup
    {
        private static readonly ILogger logger = LogManager.GetLogger();
        
        public static IPlayniteAPI Api { get; private set; }

        private RecentActivitySettingsViewModel settings { get; set; }

        public override Guid Id { get; } = Guid.Parse("7c625c8a-5a23-42db-9b9b-c90657279277");
        
        private IRecentActivityReceiver _recentActivityReceiver;

        private const int DefaultDaysToLookBack = 14;
        private DateTime _startDate;
        private DateTime _endDate;
        private SortOption _sorting;
        
        private CancellationTokenSource _cancellationTokenSource;

        public RecentActivity(IPlayniteAPI api) : base(api)
        {
            settings = new RecentActivitySettingsViewModel(this);
            Properties = new GenericPluginProperties
            {
                HasSettings = true
            };
            Api = api;
            
            _startDate = DateTime.Now.AddDays(-DefaultDaysToLookBack);
            _endDate = DateTime.Now;
            _sorting = SortOption.LastPlayed;
        }

        public override IEnumerable<SidebarItem> GetSidebarItems()
        {
            yield return new SidebarItem
            {
                Title = ResourceProvider.GetString("LOC_RecentActivity_SidebarTitle"),
                Icon = new TextBlock
                {
                    Text = char.ConvertFromUtf32(0xeffe),
                    FontFamily = ResourceProvider.GetResource("FontIcoFont") as FontFamily
                },
                Type = SiderbarItemType.View,
                Opened = () =>
                {
                    var mainView = new MainView(_startDate, _endDate,  _sorting, this);
                    _recentActivityReceiver = mainView;
                    return mainView;
                }
            };
        }
        
        public void RefreshData()
        {
            // Cancel any previously running task
            _cancellationTokenSource?.Cancel();
            // Create a new CancellationTokenSource for the current task
            _cancellationTokenSource = new CancellationTokenSource();
            
            Task.Run(async () =>
            {
                try
                {
                    var recentActivities = await RecentActivityAggregator.GetRecentActivity(
                        PlayniteApi,
                        _startDate, 
                        _endDate,
                        _cancellationTokenSource.Token
                    );
                    var activities = SortRecentActivities(recentActivities, _sorting);
                    _recentActivityReceiver?.OnRecentActivityUpdated(activities);

                }
                catch (Exception e)
                {
                }
            });
        }

        private IReadOnlyCollection<RecentActivityData> SortRecentActivities(IReadOnlyCollection<RecentActivityData> activities, SortOption sorting)
        {
            switch (_sorting)
            {
                case SortOption.GameNameAscending: return activities.OrderBy(a => a.Game.Name).ToList();
                case SortOption.GameNameDescending: return activities.OrderByDescending(a => a.Game.Name).ToList();
                case SortOption.Playtime: return activities.OrderByDescending(a => a.Playtime).ToList();
                case SortOption.LastPlayed: return activities.OrderByDescending(a => a.LastPlayed).ToList();
                case SortOption.Sessions: return activities.OrderByDescending(a => a.SessionCount).ToList();
                default: return activities;
            }
        }

        public void SetStartDate(DateTime startDate)
        {
            _startDate = startDate;
            RefreshData();
        }

        public void SetEndDate(DateTime endDate)
        {
            _endDate = endDate;
            RefreshData();
        }

        public void SetSorting(SortOption sorting)
        {
            _sorting = sorting;
            RefreshData();
        }

        public override void OnGameInstalled(OnGameInstalledEventArgs args)
        {
            // Add code to be executed when game is finished installing.
        }

        public override void OnGameStarted(OnGameStartedEventArgs args)
        {
            // Add code to be executed when game is started running.
        }

        public override void OnGameStarting(OnGameStartingEventArgs args)
        {
            // Add code to be executed when game is preparing to be started.
        }

        public override void OnGameStopped(OnGameStoppedEventArgs args)
        {
            RefreshData();
        }

        public override void OnGameUninstalled(OnGameUninstalledEventArgs args)
        {
            // Add code to be executed when game is uninstalled.
        }

        public override void OnApplicationStarted(OnApplicationStartedEventArgs args)
        {
            RefreshData();
        }

        public override void OnApplicationStopped(OnApplicationStoppedEventArgs args)
        {
            // Add code to be executed when Playnite is shutting down.
        }

        public override void OnLibraryUpdated(OnLibraryUpdatedEventArgs args)
        {
            // Add code to be executed when library is updated.
        }

        public override ISettings GetSettings(bool firstRunSettings)
        {
            return settings;
        }

        public override UserControl GetSettingsView(bool firstRunSettings)
        {
            return new RecentActivitySettingsView();
        }
    }
}