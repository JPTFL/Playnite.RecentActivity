using System;
using System.Collections.Generic;
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

            // Add sidebar panel item
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
                    var mainView = new MainView(_startDate, _endDate, this);
                    _recentActivityReceiver = mainView;
                    return mainView;
                }
            };
        }
        
        public void RefreshData()
        {
            Task.Run(async () =>
            {
                try
                {
                    var recentActivities = await RecentActivityAggregator.GetRecentActivity(
                        PlayniteApi,
                        _startDate, 
                        _endDate
                    );
                    _recentActivityReceiver?.OnRecentActivityUpdated(recentActivities);

                }
                catch (Exception e)
                {
                }
            });
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