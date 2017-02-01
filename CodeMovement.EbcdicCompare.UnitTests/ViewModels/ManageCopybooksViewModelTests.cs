using Microsoft.VisualStudio.TestTools.UnitTesting;
using CodeMovement.EbcdicCompare.UnitTests;
using CodeMovement.EbcdicCompare.Presentation.ViewModel;

namespace CodeMovement.EbcdicCompare.Tests.ViewModels
{
    [TestClass]
    public class ManageCopybooksViewModelTests
    {
        [TestMethod]
        public void Does_ManageCopybookViewModel_Reset_Properly()
        {
            var copybookManager = TestHelper.CopybookManagerMock;
            var regionManager = TestHelper.RegionManagerMock;
            var fileDialogManager = TestHelper.FileDialogInteractionMock;
            var fileOperationsManager = TestHelper.FileOperationsManagerMock;
            var eventAggregator = TestHelper.CreateEventAggregator();

            var manageCopybookManager = new ManageCopybooksViewModel(eventAggregator, regionManager, 
                fileDialogManager, fileOperationsManager, copybookManager);


        }
    }
}
