using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Events;
using Prism.Regions;
using Rhino.Mocks;
using CodeMovement.EbcdicCompare.Models.Constant;
using CodeMovement.EbcdicCompare.Presentation.ViewModel;

namespace CodeMovement.EbcdicCompare.Tests.ViewModels
{
    [TestClass]
    public class HomeViewModelTests
    {
        #region "Mock Helpers"

        public IRegionManager RegionManagerMock
        {
            get { return MockRepository.GenerateMock<IRegionManager>(); }
        }

        public IEventAggregator EventAggregatorStub
        {
            get { return MockRepository.GenerateStub<IEventAggregator>(); }
        }

        #endregion

        [TestMethod]
        public void HomeViewModel_Go_To_Open_Ebcdic_File_View()
        {
            var regionManagerMock = RegionManagerMock;
            regionManagerMock.Expect(
                m => m.RequestNavigate(RegionNames.MainContentRegion, ViewIdentity.OpenEbcdicFileViewUrl));

            var viewModel = new HomeViewModel(regionManagerMock, EventAggregatorStub);
            viewModel.OpenEbcdicFile.Execute(null);

            regionManagerMock.VerifyAllExpectations();
        }

        [TestMethod]
        public void HomeViewModel_Go_To_Compare_Ebcdic_Files_View()
        {
            var regionManagerMock = RegionManagerMock;
            regionManagerMock.Expect(
                m => m.RequestNavigate(RegionNames.MainContentRegion, ViewIdentity.CompareEbcdicFilesViewUrl));

            var viewModel = new HomeViewModel(regionManagerMock, EventAggregatorStub);
            viewModel.CompareEbcdicFiles.Execute(null);

            regionManagerMock.VerifyAllExpectations();
        }

        [TestMethod]
        public void HomeViewModel_Go_To_Manage_Copybooks_View()
        {
            var regionManagerMock = RegionManagerMock;
            regionManagerMock.Expect(
                m => m.RequestNavigate(RegionNames.MainContentRegion, ViewIdentity.ManageCopybookXmlFilesViewUrl));

            var viewModel = new HomeViewModel(regionManagerMock, EventAggregatorStub);
            viewModel.ManageCopybooks.Execute(null);

            regionManagerMock.VerifyAllExpectations();
        }
    }
}
