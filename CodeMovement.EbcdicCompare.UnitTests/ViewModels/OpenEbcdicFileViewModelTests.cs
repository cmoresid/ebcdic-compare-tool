using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Events;
using Prism.Regions;
using Rhino.Mocks;
using CodeMovement.EbcdicCompare.Models.Result;
using CodeMovement.EbcdicCompare.Services;
using CodeMovement.EbcdicCompare.Presentation.Event;
using CodeMovement.EbcdicCompare.Presentation.Interaction;
using CodeMovement.EbcdicCompare.Presentation.ViewModel;

namespace CodeMovement.EbcdicCompare.Tests.ViewModels
{
    [TestClass]
    public class OpenEbcdicFileViewModelTests
    {
        public const string EbcdicFilePath = @"C:\My\EBCDICFILE";
        public const string EbcdicFileName = "EBCDICFILE";
        public const string CopybookFilePath = @"C:\My\COPYBOOK.fileformat";

        #region "Mock Helpers"

        public IEventAggregator EventAggregatorStub
        {
            get
            {
                var eventAggregatorMock = MockRepository.GenerateMock<IEventAggregator>();
                var finishReadEbcdicFileEventMock = new FinishReadEbcdicFileEventMock();
                var viewEbcdicFileRequestEventMock = new ViewEbcdicFileRequestEventMock();

                eventAggregatorMock.Stub(m => m.GetEvent<FinishReadEbcdicFileEvent>())
                    .Return(finishReadEbcdicFileEventMock).Repeat.Any();

                eventAggregatorMock.Stub(m => m.GetEvent<ViewEbcdicFileRequestEvent>())
                    .Return(viewEbcdicFileRequestEventMock).Repeat.Any();

                return eventAggregatorMock;
            }
        }

        public IRegionManager RegionManagerMock
        {
            get { return MockRepository.GenerateMock<IRegionManager>(); }
        }

        public ICopybookManager CopybookManagerMock
        {
            get { return MockRepository.GenerateMock<ICopybookManager>(); }
        }

        public IFileDialogInteraction FileDialogInteractionMock
        {
            get { return MockRepository.GenerateMock<IFileDialogInteraction>(); }
        }

        #endregion

        [TestMethod]
        public void Can_OpenEbcdicFileViewModel_View_Ebcdic_File_No_Fields_Set()
        {
            var viewModel = new OpenEbcdicFileViewModel(EventAggregatorStub, 
                RegionManagerMock, CopybookManagerMock, FileDialogInteractionMock);

            Assert.IsFalse(viewModel.ReadEbcdicFile.CanExecute());
        }

        [TestMethod]
        public void Can_OpenEbcdicFileViewModel_View_Ebcdic_File_Ebcdic_File_Field_Set()
        {
            var copybookManagerMock = CopybookManagerMock;
            copybookManagerMock.Stub(m => m.GetCopybookPathForEbcdicFile(EbcdicFileName)).Return(new OperationResult<string>
            {
                Result = null
            });

            var fileInteractionMock = FileDialogInteractionMock;
            fileInteractionMock.Stub(m => m.OpenFileDialog("Select EBCDIC File")).Return(EbcdicFilePath);

            var viewModel = new OpenEbcdicFileViewModel(EventAggregatorStub,
                RegionManagerMock, copybookManagerMock, fileInteractionMock);

            viewModel.SelectEbcdicFile.Execute(null);

            Assert.IsFalse(viewModel.ReadEbcdicFile.CanExecute());
        }

        [TestMethod]
        public void Can_OpenEbcdicFileViewModel_View_Ebcdic_File_Copybook_File_Field_Set()
        {
            var fileInteractionMock = FileDialogInteractionMock;
            fileInteractionMock.Stub(m => m.OpenFileDialog("Select Copybook XML File")).Return(CopybookFilePath);

            var viewModel = new OpenEbcdicFileViewModel(EventAggregatorStub,
                RegionManagerMock, CopybookManagerMock, fileInteractionMock);

            viewModel.SelectCopybookFile.Execute(null);

            Assert.IsFalse(viewModel.ReadEbcdicFile.CanExecute());
        }

        [TestMethod]
        public void Does_OpenEbcdicFileViewModel_Reset_Fields_Properly()
        {
            var copybookManagerMock = CopybookManagerMock;
            copybookManagerMock.Stub(m => m.GetCopybookPathForEbcdicFile(EbcdicFileName)).Return(new OperationResult<string>
            {
                Result = null
            }).Repeat.Any();

            var viewModel = new OpenEbcdicFileViewModel(EventAggregatorStub,
                RegionManagerMock, copybookManagerMock, FileDialogInteractionMock);

            viewModel.EbcdicFilePath = EbcdicFilePath;
            viewModel.CopybookFilePath = CopybookFilePath;

            viewModel.ResetView.Execute(null);

            Assert.IsNull(viewModel.EbcdicFilePath);
            Assert.IsNull(viewModel.CopybookFilePath);
            Assert.IsFalse(viewModel.IsLoadingEbcdicFile);
            Assert.IsFalse(viewModel.ReadEbcdicFile.CanExecute());
        }

        [TestMethod]
        public void Can_OpenEbcdicFileViewModel_View_Ebcdic_File_Fields_Set()
        {
            var finishReadEbcdicFileEventMock = new FinishReadEbcdicFileEventMock();
            var viewEbcdicFileRequestEventMock = new ViewEbcdicFileRequestEventMock();

            var eventAggregatorMock = MockRepository.GenerateMock<IEventAggregator>();
            eventAggregatorMock.Stub(m => m.GetEvent<FinishReadEbcdicFileEvent>())
                    .Return(finishReadEbcdicFileEventMock).Repeat.Any();
            eventAggregatorMock.Expect(m => m.GetEvent<ViewEbcdicFileRequestEvent>())
                    .Return(viewEbcdicFileRequestEventMock).Repeat.Any();

            var copybookManagerMock = CopybookManagerMock;
            copybookManagerMock.Stub(m => m.GetCopybookPathForEbcdicFile(EbcdicFileName)).Return(new OperationResult<string>
            {
                Result = null
            });

            var fileInteractionMock = FileDialogInteractionMock;
            fileInteractionMock.Stub(m => m.OpenFileDialog("Select EBCDIC File")).Return(EbcdicFilePath);
            fileInteractionMock.Stub(m => m.OpenFileDialog("Select Copybook XML File")).Return(CopybookFilePath);

            var viewModel = new OpenEbcdicFileViewModel(eventAggregatorMock,
                RegionManagerMock, copybookManagerMock, fileInteractionMock);

            viewModel.SelectEbcdicFile.Execute(null);
            viewModel.SelectCopybookFile.Execute(null);

            Assert.IsTrue(viewModel.ReadEbcdicFile.CanExecute());

            viewModel.ReadEbcdicFile.Execute();
            Assert.IsTrue(viewModel.IsLoadingEbcdicFile);

            eventAggregatorMock.GetEvent<FinishReadEbcdicFileEvent>().Publish(new FinishReadEbcdicFile());
            Assert.IsFalse(viewModel.IsLoadingEbcdicFile);

            eventAggregatorMock.VerifyAllExpectations();
        }
    }
}
