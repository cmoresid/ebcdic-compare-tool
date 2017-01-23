using System;
using System.Windows;
using Prism.Events;
using Prism.Modularity;
using Prism.Regions;
using CodeMovement.EbcdicCompare.Models;
using CodeMovement.EbcdicCompare.Models.Constant;
using CodeMovement.EbcdicCompare.Presentation.Event;

namespace CodeMovement.EbcdicCompare.App
{
    /// <summary>
    /// Interaction logic for Shell.xaml
    /// </summary>
    public partial class Shell : Window
    {
        private const string PresentationModule = "PresentationModule";
        private static readonly Uri HomeViewUri = new Uri("/HomeView", UriKind.Relative);

        private readonly IRegionManager _regionManager;
        private readonly IModuleManager _moduleManager;
        private readonly IEventAggregator _eventAggregator;

        public Shell(IRegionManager regionManager, 
            IModuleManager moduleManager, 
            IEventAggregator eventAggregator)
        {
            _regionManager = regionManager;
            _moduleManager = moduleManager;
            _eventAggregator = eventAggregator;

            InitializeComponent();
            SubscribeToWindowResizeEvent();

            _moduleManager.LoadModuleCompleted +=
                (s, e) =>
                {
                    if (e.ModuleInfo.ModuleName == PresentationModule)
                    {
                        _regionManager.RequestNavigate(RegionNames.MainContentRegion, HomeViewUri);
                    }
                };
        }

        #region "Private Methods"

        private void ResizeWindowHandler(WindowSize e)
        {
            Width = e.Width;
            Height = e.Height;
        }

        private void SubscribeToWindowResizeEvent()
        {
            _eventAggregator.GetEvent<WindowResizeEvent>().Subscribe(ResizeWindowHandler, ThreadOption.UIThread, false);
        }

        #endregion
    }
}
