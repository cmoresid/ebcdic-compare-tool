using System;
using System.Windows.Input;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using CodeMovement.EbcdicCompare.Models;
using CodeMovement.EbcdicCompare.Models.Constant;
using CodeMovement.EbcdicCompare.Presentation.Event;

namespace CodeMovement.EbcdicCompare.Presentation.ViewModel
{
    public abstract class BaseNavigationViewModel : BindableBase, INavigationAware
    {
        #region "Attributes"

        private const string HomeViewKey = "HomeView";
        private static readonly Uri HomeViewUrl = new Uri(HomeViewKey, UriKind.Relative);

        protected readonly IRegionManager RegionManager;
        protected readonly IEventAggregator EventAggregator;

        #endregion

        protected BaseNavigationViewModel(IRegionManager regionManager,
            IEventAggregator eventAggregator,
            WindowSize windowSize)
        {
            RegionManager = regionManager;
            EventAggregator = eventAggregator;

            OpenHomeView = new DelegateCommand(OnOpenHomeView);
            ResetView = new DelegateCommand(OnResetView);

            ViewWindowSize = windowSize;
        }

        protected WindowSize ViewWindowSize { get; set; }

        public ICommand OpenHomeView { get; private set; }
        public ICommand ResetView { get; private set; }

        protected abstract void OnResetView();

        #region "INavigationAware"

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            EventAggregator.GetEvent<WindowResizeEvent>().Publish(ViewWindowSize);
        }

        #endregion

        #region "Event Handlers"

        private void OnOpenHomeView()
        {
            RegionManager.RequestNavigate(RegionNames.MainContentRegion, HomeViewUrl);
        }

        #endregion
    }
}
