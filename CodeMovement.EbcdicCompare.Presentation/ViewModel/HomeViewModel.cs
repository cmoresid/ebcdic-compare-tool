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
    public class HomeViewModel : BindableBase, INavigationAware
    {
        #region "Attributes"

        private static readonly WindowSize HomeViewWindowSize = new WindowSize(340, 280);

        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;

        #endregion

        public HomeViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;

            OpenEbcdicFile = new DelegateCommand(OnOpenEbcdicFileView);
            CompareEbcdicFiles = new DelegateCommand(OnCompareEbcdicFilesView);
            ManageCopybooks = new DelegateCommand(OnManageCopybookXmlFilesView);
        }

        public ICommand OpenEbcdicFile { get; private set; }
        public ICommand CompareEbcdicFiles { get; private set; }
        public ICommand ManageCopybooks { get; private set; }

        #region "Event Handlers"

        private void OnOpenEbcdicFileView()
        {
            _regionManager.RequestNavigate(RegionNames.MainContentRegion, ViewIdentity.OpenEbcdicFileViewUrl);
        }

        private void OnCompareEbcdicFilesView()
        {
            _regionManager.RequestNavigate(RegionNames.MainContentRegion, ViewIdentity.CompareEbcdicFilesViewUrl);
        }

        private void OnManageCopybookXmlFilesView()
        {
            _regionManager.RequestNavigate(RegionNames.MainContentRegion, ViewIdentity.ManageCopybookXmlFilesViewUrl);
        }

        #endregion

        #region "INavigationAware"

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            // Does not need to handle OnNavigatedFrom delegate method.
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _eventAggregator.GetEvent<WindowResizeEvent>().Publish(HomeViewWindowSize);
        }

        #endregion
    }
}
