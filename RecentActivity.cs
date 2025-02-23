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

        private RecentActivitySettingsViewModel Settings { get; set; }
        
        public override Guid Id { get; } = Guid.Parse("7c625c8a-5a23-42db-9b9b-c90657279277");
        
        private IRecentActivityReceiver _recentActivityReceiver;
        private readonly RecentActivityAggregator _recentActivityAggregator;
        private readonly SemaphoreSlim _refreshLock = new SemaphoreSlim(1, 1);

        private const int DefaultDaysToLookBack = 14;
        private DateTime _startDate;
        private DateTime _endDate;
        private SortOption _sorting;

        public RecentActivity(IPlayniteAPI api) : base(api)
        {
            Settings = new RecentActivitySettingsViewModel(this);
            Properties = new GenericPluginProperties
            {
                HasSettings = false
            };
            Api = api;
            
            _startDate = DateTime.Now.AddDays(-DefaultDaysToLookBack);
            _endDate = DateTime.Now;
            _sorting = SortOption.LastPlayed;
            _recentActivityAggregator = new RecentActivityAggregator(Api);
        }

        public override IEnumerable<SidebarItem> GetSidebarItems()
        {
            yield return new SidebarItem
            {
                Title = ResourceProvider.GetString("LOC_RecentActivity_SidebarTitle"),
                Icon = new TextBlock
                {
                    Text = char.ConvertFromUtf32(0xeff0),
                    FontFamily = ResourceProvider.GetResource("FontIcoFont") as FontFamily
                },
                Type = SiderbarItemType.View,
                Opened = () =>
                {
                    var mainView = new MainView(_startDate, _endDate,  _sorting, this, Api);
                    _recentActivityReceiver = mainView;
                    RefreshData();
                    return mainView;
                }
            };
        }
        
        public void RefreshData(bool refreshGamesFromApi = false)
        {
            // only allow one refresh at a time and cancel if another refresh is already in progress
            if (!_refreshLock.Wait(0))
            {
                return;
            }

            Task.Run(async () =>
            {
                try
                {
                    if (refreshGamesFromApi)
                    {
                        await _recentActivityAggregator.LoadActivityData();
                    }

                    if (_recentActivityReceiver != null)
                    {
                        var recentActivities = _recentActivityAggregator.GetRecentActivity(
                            PlayniteApi,
                            _startDate, 
                            _endDate
                        );
                        var activities = SortRecentActivities(recentActivities, _sorting);
                        _recentActivityReceiver.OnRecentActivityUpdated(activities);
                    }
                }
                finally
                {
                    // Release refresh lock
                    _refreshLock.Release();
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
        }

        public override void OnGameUninstalled(OnGameUninstalledEventArgs args)
        {
            // Add code to be executed when game is uninstalled.
        }

        public override void OnApplicationStarted(OnApplicationStartedEventArgs args)
        {
            RefreshData(true);
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
            return Settings;
        }

        public override UserControl GetSettingsView(bool firstRunSettings)
        {
            return new RecentActivitySettingsView();
        }
    }
}