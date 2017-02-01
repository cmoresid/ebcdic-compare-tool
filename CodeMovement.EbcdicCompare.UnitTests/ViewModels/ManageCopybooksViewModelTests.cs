using Microsoft.VisualStudio.TestTools.UnitTesting;
using CodeMovement.EbcdicCompare.UnitTests;
using CodeMovement.EbcdicCompare.Presentation.ViewModel;
using Rhino.Mocks;
using CodeMovement.EbcdicCompare.Models;
using System.Collections.Generic;
using CodeMovement.EbcdicCompare.Models.Result;

namespace CodeMovement.EbcdicCompare.Tests.ViewModels
{
    [TestClass]
    public class ManageCopybooksViewModelTests
    {
        [TestMethod]
        public void Does_ManageCopybookViewModel_Initialize_Properly()
        {
            var copybookManager = TestHelper.CopybookManagerMock;
            var regionManager = TestHelper.RegionManagerMock;
            var fileDialogManager = TestHelper.FileDialogInteractionMock;
            var fileOperationsManager = TestHelper.FileOperationsManagerMock;
            var eventAggregator = TestHelper.CreateEventAggregator();

            var manageCopybookManager = new ManageCopybooksViewModel(eventAggregator, regionManager,
                fileDialogManager, fileOperationsManager, copybookManager);

            Assert.IsFalse(manageCopybookManager.ViewCopybookCommand.CanExecute());
            Assert.IsFalse(manageCopybookManager.DeleteExistingCopybookCommand.CanExecute());
            Assert.IsFalse(manageCopybookManager.DeleteExistingEbcdicFileCommand.CanExecute());
            Assert.IsTrue(manageCopybookManager.SelectNewCopybookCommand.CanExecute());
            Assert.IsTrue(manageCopybookManager.SelectNewEbcdicFileCommand.CanExecute());
            Assert.IsNull(manageCopybookManager.SelectedNewEbcdicFile);
            Assert.IsNull(manageCopybookManager.SelectedNewCopybookFile);
        }

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

            manageCopybookManager.SelectedExistingEbcdicFile = @"C:\Some\File.txt";
            manageCopybookManager.ResetView.Execute(null);

            Assert.IsFalse(manageCopybookManager.ViewCopybookCommand.CanExecute());
            Assert.IsFalse(manageCopybookManager.DeleteExistingCopybookCommand.CanExecute());
            Assert.IsFalse(manageCopybookManager.DeleteExistingEbcdicFileCommand.CanExecute());
            Assert.IsTrue(manageCopybookManager.SelectNewCopybookCommand.CanExecute());
            Assert.IsTrue(manageCopybookManager.SelectNewEbcdicFileCommand.CanExecute());
            Assert.IsNull(manageCopybookManager.SelectedNewEbcdicFile);
            Assert.IsNull(manageCopybookManager.SelectedNewCopybookFile);
        }

        [TestMethod]
        public void Can_ManageCopybookViewModel_View_Or_Delete_Selected_Copybook()
        {
            var associations = new List<CopybookAssociation>()
            {
                new CopybookAssociation() { FilePath = @"C:\Copybook1", AssociatedFiles = new List<string> { @"C:\EBCDICFILE1.txt" } }
            };

            var copybookManager = TestHelper.CopybookManagerMock;
            copybookManager.Expect(m => m.GetCopybooks()).Return(associations);

            var regionManager = TestHelper.RegionManagerMock;
            var fileDialogManager = TestHelper.FileDialogInteractionMock;
            var fileOperationsManager = TestHelper.FileOperationsManagerMock;
            var eventAggregator = TestHelper.CreateEventAggregator();

            var manageCopybookManager = new ManageCopybooksViewModel(eventAggregator, regionManager,
                fileDialogManager, fileOperationsManager, copybookManager);

            manageCopybookManager.SelectedCopybook = associations[0];
            Assert.IsTrue(manageCopybookManager.ViewCopybookCommand.CanExecute());
            Assert.IsTrue(manageCopybookManager.DeleteExistingCopybookCommand.CanExecute());
        }

        [TestMethod]
        public void Can_ManageCopybookViewModel_Delete_Selected_Copybook()
        {
            var associations = new List<CopybookAssociation>()
            {
                new CopybookAssociation() { FilePath = @"C:\Copybook1", AssociatedFiles = new List<string> { @"C:\EBCDICFILE1.txt" } }
            };

            var copybookManager = TestHelper.CopybookManagerMock;
            copybookManager.Expect(m => m.GetCopybooks()).Return(associations);
            copybookManager.Expect(m => m.DeleteCopybook(@"C:\Copybook1"));

            var regionManager = TestHelper.RegionManagerMock;
            var fileDialogManager = TestHelper.FileDialogInteractionMock;
            var fileOperationsManager = TestHelper.FileOperationsManagerMock;
            var eventAggregator = TestHelper.CreateEventAggregator();

            var manageCopybookManager = new ManageCopybooksViewModel(eventAggregator, regionManager,
                fileDialogManager, fileOperationsManager, copybookManager);

            manageCopybookManager.SelectedCopybook = associations[0];
            Assert.IsTrue(manageCopybookManager.DeleteExistingCopybookCommand.CanExecute());

            manageCopybookManager.DeleteExistingCopybookCommand.Execute();

            copybookManager.VerifyAllExpectations();
        }

        [TestMethod]
        public void Can_ManageCopybookViewModel_Delete_Selected_Ebcdic_File()
        {
            var associations = new List<CopybookAssociation>()
            {
                new CopybookAssociation() { FilePath = @"C:\Copybook1", AssociatedFiles = new List<string> { @"C:\EBCDICFILE1.txt" } }
            };

            var copybookManager = TestHelper.CopybookManagerMock;
            copybookManager.Expect(m => m.GetCopybooks()).Return(associations);
            copybookManager.Expect(m => m.DeleteCopybookFileAssociation(@"C:\Copybook1", @"C:\EBCDICFILE1.txt"));

            var regionManager = TestHelper.RegionManagerMock;
            var fileDialogManager = TestHelper.FileDialogInteractionMock;
            var fileOperationsManager = TestHelper.FileOperationsManagerMock;
            var eventAggregator = TestHelper.CreateEventAggregator();

            var manageCopybookManager = new ManageCopybooksViewModel(eventAggregator, regionManager,
                fileDialogManager, fileOperationsManager, copybookManager);

            manageCopybookManager.SelectedCopybook = associations[0];
            manageCopybookManager.SelectedExistingEbcdicFile = 
                manageCopybookManager.SelectedCopybook.AssociatedFiles[0];

            Assert.IsTrue(manageCopybookManager.DeleteExistingEbcdicFileCommand.CanExecute());
            manageCopybookManager.DeleteExistingEbcdicFileCommand.Execute();

            copybookManager.VerifyAllExpectations();
        }

        [TestMethod]
        public void Can_ManageCopybookViewModel_Associate_Copybook_With_File()
        {
            const string copybookFile = @"C:\Copybook.fileformat";
            const string ebcdicFile = @"C:\EBCDICFILE.txt";

            var copybookManager = TestHelper.CopybookManagerMock;
            copybookManager.Expect(m => m.AddCopybookEbcdicFileAssociation(copybookFile, "EBCDICFILE.txt")).Return(new OperationResult<bool>
            {
                Result = true
            });
            copybookManager.Expect(m => m.GetCopybooks()).Return(new List<CopybookAssociation>());

            var regionManager = TestHelper.RegionManagerMock;
            var fileDialogManager = TestHelper.FileDialogInteractionMock;
            fileDialogManager.Expect(m => m.OpenFileDialog("Select New Copybook XML file")).Return(copybookFile);
            fileDialogManager.Expect(m => m.OpenFileDialog("Select New EBCDIC file")).Return(ebcdicFile);

            var fileOperationsManager = TestHelper.FileOperationsManagerMock;
            var eventAggregator = TestHelper.CreateEventAggregator();

            var manageCopybookManager = new ManageCopybooksViewModel(eventAggregator, regionManager,
                fileDialogManager, fileOperationsManager, copybookManager);

            manageCopybookManager.SelectNewCopybookCommand.Execute();
            manageCopybookManager.SelectNewEbcdicFileCommand.Execute();

            Assert.IsTrue(manageCopybookManager.AddAssociationCommand.CanExecute());

            manageCopybookManager.AddAssociationCommand.Execute();

            copybookManager.VerifyAllExpectations();
            fileDialogManager.VerifyAllExpectations();
        }
    }
}
