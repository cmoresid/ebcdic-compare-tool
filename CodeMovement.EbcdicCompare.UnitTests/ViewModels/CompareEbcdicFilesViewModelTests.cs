using CodeMovement.EbcdicCompare.Models.Constant;
using CodeMovement.EbcdicCompare.Models.Result;
using CodeMovement.EbcdicCompare.Presentation.ViewModel;
using CodeMovement.EbcdicCompare.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Events;
using Rhino.Mocks;
using System;
using System.Threading.Tasks;

namespace CodeMovement.EbcdicCompare.Tests.ViewModels
{
    [TestClass]
    public class CompareEbcdicFilesViewModelTests
    {
        [TestMethod]
        public void CompareEbcdicFilesViewModel_Initialize()
        {
            var finishEvent = MockRepository.GenerateMock<FinishReadEbcdicFileEventMock>();
            finishEvent.Expect(m => m.Subscribe(Arg<Action<FinishReadEbcdicFile>>.Is.Anything,
                Arg<ThreadOption>.Is.Anything, Arg<bool>.Is.Anything, Arg<Predicate<FinishReadEbcdicFile>>.Is.Anything))
                .Return(new SubscriptionToken(e => { }));

            var eventAggregator = TestHelper.CreateEventAggregator(finishReadEvent: finishEvent);

            var viewModel = new CompareEbcdicFilesViewModel(eventAggregator, TestHelper.RegionManagerMock,
                TestHelper.FileDialogInteractionMock, TestHelper.CompareEbcdicFilesServiceMock,
                TestHelper.CopybookManagerMock, TestHelper.ExternalProgramServiceMock);

            Assert.IsNotNull(viewModel.ErrorConfirmationRequest);
            Assert.IsNotNull(viewModel.SelectLegacyFile);
            Assert.IsNotNull(viewModel.SelectModernizedFile);
            Assert.IsNotNull(viewModel.PerformInitialCompare);
            Assert.IsNotNull(viewModel.SelectUseCopybook);
            Assert.IsNotNull(viewModel.OpenInExternalProgram);
            Assert.IsNotNull(viewModel.SelectCopybook);
            Assert.IsNotNull(viewModel.PerformCopybookCompare);
            Assert.AreEqual(CompareEbcdicFilesViewModel.States.Initial, viewModel.CurrentState);

            Assert.IsTrue(viewModel.SelectCopybook.CanExecute(null));
            Assert.IsTrue(viewModel.SelectLegacyFile.CanExecute(null));
            Assert.IsFalse(viewModel.PerformInitialCompare.CanExecute());

            finishEvent.VerifyAllExpectations();
        }

        [TestMethod]
        public async Task Can_CompareEbcdicFilesViewModel_Perform_Initial_Compare_Files_Match()
        {
            const string file1 = @"C:\FILE1.txt";
            const string file2 = @"C:\FILE2.txt";

            var finishEvent = new FinishReadEbcdicFileEventMock();
            var eventAggregator = TestHelper.CreateEventAggregator(finishReadEvent: finishEvent);

            var compareService = TestHelper.CompareEbcdicFilesServiceMock;
            compareService.Expect(m => m.CompareEbcdicByteContents(file1, file2)).Return(new OperationResult<CompareEbcdicFileResult>
            {
                Result = new CompareEbcdicFileResult
                {
                    AreIdentical = true,
                    FirstEbcdicFile = new EbcdicFileAnalysis(),
                    SecondEbcdicFile = new EbcdicFileAnalysis()
                }
            });

            var fileInteraction = TestHelper.FileDialogInteractionMock;
            fileInteraction.Expect(m => m.OpenFileDialog("Select Legacy EBCDIC file")).Return(file1);
            fileInteraction.Expect(m => m.OpenFileDialog("Select Modernized EBCDIC file")).Return(file2);

            var viewModel = new CompareEbcdicFilesViewModel(eventAggregator, TestHelper.RegionManagerMock,
                fileInteraction, compareService,
                TestHelper.CopybookManagerMock, TestHelper.ExternalProgramServiceMock);

            Assert.IsFalse(viewModel.PerformInitialCompare.CanExecute());

            viewModel.SelectLegacyFile.Execute(null);
            Assert.AreEqual(file1, viewModel.LegacyEbcdicFilePath);

            viewModel.SelectModernizedFile.Execute(null);
            Assert.AreEqual(file2, viewModel.ModernizedEbcdicFilePath);

            Assert.IsTrue(viewModel.PerformInitialCompare.CanExecute());
            await viewModel.PerformInitialCompare.Execute();

            Assert.AreEqual(CompareEbcdicFilesViewModel.States.FilesMatch, viewModel.CurrentState);
            Assert.IsTrue(viewModel.ShowFilesMatchLabel);
            Assert.IsFalse(viewModel.ShowFilesDoNotMatchLabel);
        }

        [TestMethod]
        public async Task Can_CompareEbcdicFilesViewModel_Perform_Initial_Compare_Files_Do_Not_Match()
        {
            const string file1 = @"C:\FILE1.txt";
            const string file2 = @"C:\FILE2.txt";

            var finishEvent = new FinishReadEbcdicFileEventMock();
            var eventAggregator = TestHelper.CreateEventAggregator(finishReadEvent: finishEvent);

            var compareService = TestHelper.CompareEbcdicFilesServiceMock;
            compareService.Expect(m => m.CompareEbcdicByteContents(file1, file2)).Return(new OperationResult<CompareEbcdicFileResult>
            {
                Result = new CompareEbcdicFileResult
                {
                    AreIdentical = false,
                    FirstEbcdicFile = new EbcdicFileAnalysis(),
                    SecondEbcdicFile = new EbcdicFileAnalysis()
                }
            });

            var fileInteraction = TestHelper.FileDialogInteractionMock;
            fileInteraction.Expect(m => m.OpenFileDialog("Select Legacy EBCDIC file")).Return(file1);
            fileInteraction.Expect(m => m.OpenFileDialog("Select Modernized EBCDIC file")).Return(file2);

            var viewModel = new CompareEbcdicFilesViewModel(eventAggregator, TestHelper.RegionManagerMock,
                fileInteraction, compareService,
                TestHelper.CopybookManagerMock, TestHelper.ExternalProgramServiceMock);

            Assert.IsFalse(viewModel.PerformInitialCompare.CanExecute());

            viewModel.SelectLegacyFile.Execute(null);
            Assert.AreEqual(file1, viewModel.LegacyEbcdicFilePath);

            viewModel.SelectModernizedFile.Execute(null);
            Assert.AreEqual(file2, viewModel.ModernizedEbcdicFilePath);

            Assert.IsTrue(viewModel.PerformInitialCompare.CanExecute());
            await viewModel.PerformInitialCompare.Execute();

            Assert.AreEqual(CompareEbcdicFilesViewModel.States.FilesDoNotMatch, viewModel.CurrentState);
            Assert.IsFalse(viewModel.ShowFilesMatchLabel);
            Assert.IsTrue(viewModel.ShowFilesDoNotMatchLabel);
            Assert.IsTrue(viewModel.ShowCompareUsingCopybookRow);

            fileInteraction.VerifyAllExpectations();
            compareService.VerifyAllExpectations();
        }

        [TestMethod]
        public void Can_CompareEbcdicFilesViewModel_Compare_Using_External_Program()
        {
            const string file1 = @"C:\FILE1.txt";
            const string file2 = @"C:\FILE2.txt";

            var finishEvent = new FinishReadEbcdicFileEventMock();
            var updateEvent = new UpdateEbcdicFileGridEventMock();
            var eventAggregator = TestHelper.CreateEventAggregator(finishReadEvent: finishEvent, updateEvent: updateEvent);

            var externalProgram = TestHelper.ExternalProgramServiceMock;
            externalProgram.Stub(m => m.RunProgram(Arg<string>.Is.Anything, Arg<object[]>.Is.Anything));

            var viewModel = new CompareEbcdicFilesViewModel(eventAggregator, TestHelper.RegionManagerMock,
                TestHelper.FileDialogInteractionMock, TestHelper.CompareEbcdicFilesServiceMock,
                TestHelper.CopybookManagerMock, externalProgram);

            // Set state to files do not match.
            viewModel.LegacyEbcdicFilePath = file1;
            viewModel.ModernizedEbcdicFilePath = file2;
            viewModel.CurrentState = CompareEbcdicFilesViewModel.States.FilesDoNotMatch;

            // Choose not to use a copybook for the comparison.
            viewModel.UseCopybook = false;
            viewModel.SelectUseCopybook.Execute(null);

            // Ensure that external program option is shown.
            Assert.AreEqual(CompareEbcdicFilesViewModel.States.DoNotUseCopybook, viewModel.CurrentState);
            Assert.IsTrue(viewModel.ShowOpenFilesInExternalEditorRow);
            Assert.IsTrue(viewModel.OpenInExternalProgram.CanExecute(null));

            viewModel.OpenInExternalProgram.Execute(null);
            Assert.AreEqual(CompareEbcdicFilesViewModel.States.Initial, viewModel.CurrentState);
        }

        [TestMethod]
        public void Can_CompareEbcdicFilesViewModel_Compare_Using_Copybook_No_Previous_Association()
        {
            const string file1 = @"C:\FILE1.txt";
            const string file2 = @"C:\FILE2.txt";

            var finishEvent = new FinishReadEbcdicFileEventMock();
            var updateEvent = new UpdateEbcdicFileGridEventMock();
            var eventAggregator = TestHelper.CreateEventAggregator(finishReadEvent: finishEvent, updateEvent: updateEvent);

            var copybookManager = TestHelper.CopybookManagerMock;
            copybookManager.Expect(m => m.GetCopybookPathForEbcdicFile(file1)).Return(new OperationResult<string>
            {
                Result = null
            });
            copybookManager.Expect(m => m.GetCopybookPathForEbcdicFile(file2)).Return(new OperationResult<string>
            {
                Result = null
            });

            var viewModel = new CompareEbcdicFilesViewModel(eventAggregator, TestHelper.RegionManagerMock,
                TestHelper.FileDialogInteractionMock, TestHelper.CompareEbcdicFilesServiceMock,
                copybookManager, TestHelper.ExternalProgramServiceMock);

            // Set state to files do not match.
            viewModel.LegacyEbcdicFilePath = file1;
            viewModel.ModernizedEbcdicFilePath = file2;
            viewModel.CurrentState = CompareEbcdicFilesViewModel.States.FilesDoNotMatch;

            // Choose not to use a copybook for the comparison.
            viewModel.UseCopybook = true;
            viewModel.SelectUseCopybook.Execute(null);

            // Ensure that external program option is shown.
            Assert.AreEqual(CompareEbcdicFilesViewModel.States.UseCopybook, viewModel.CurrentState);
            Assert.IsTrue(viewModel.ShowCompareUsingCopybookRow);
            Assert.IsTrue(viewModel.SelectCopybook.CanExecute(true));

            copybookManager.VerifyAllExpectations();
        }

        [TestMethod]
        public void Can_CompareEbcdicFilesViewModel_Compare_Using_Copybook_Previous_Association()
        {
            const string file1 = @"C:\FILE1.txt";
            const string file2 = @"C:\FILE2.txt";
            const string copybook = @"C:\Copybook.fileformat";

            var finishEvent = new FinishReadEbcdicFileEventMock();
            var updateEvent = new UpdateEbcdicFileGridEventMock();
            var eventAggregator = TestHelper.CreateEventAggregator(finishReadEvent: finishEvent, updateEvent: updateEvent);

            var copybookManager = TestHelper.CopybookManagerMock;
            copybookManager.Stub(m => m.GetCopybookPathForEbcdicFile(file1)).Return(new OperationResult<string>
            {
                Result = copybook
            });
            copybookManager.Stub(m => m.GetCopybookPathForEbcdicFile(file2)).Return(new OperationResult<string>
            {
                Result = copybook
            });

            var viewModel = new CompareEbcdicFilesViewModel(eventAggregator, TestHelper.RegionManagerMock,
                TestHelper.FileDialogInteractionMock, TestHelper.CompareEbcdicFilesServiceMock,
                copybookManager, TestHelper.ExternalProgramServiceMock);

            // Set state to files do not match.
            viewModel.LegacyEbcdicFilePath = file1;
            viewModel.ModernizedEbcdicFilePath = file2;
            viewModel.CurrentState = CompareEbcdicFilesViewModel.States.FilesDoNotMatch;

            // Choose not to use a copybook for the comparison.
            viewModel.UseCopybook = true;
            viewModel.SelectUseCopybook.Execute(null);

            // Ensure that external program option is shown.
            Assert.AreEqual(CompareEbcdicFilesViewModel.States.ReadyToPerformCopybookCompare, viewModel.CurrentState);
            Assert.IsFalse(viewModel.ShowSelectCopybookRow);
            Assert.IsTrue(viewModel.PerformCopybookCompare.CanExecute());
        }

        [TestMethod]
        public void CompareEbcdicFilesViewModel_Perform_Copybook_Compare()
        {
            const string file1 = @"C:\FILE1.txt";
            const string file2 = @"C:\FILE2.txt";
            const string copybook = @"C:\Copybook.fileformat";

            var finishEvent = new FinishReadEbcdicFileEventMock();
            var updateEvent = new UpdateEbcdicFileGridEventMock();
            var eventAggregator = TestHelper.CreateEventAggregator(finishReadEvent: finishEvent, updateEvent: updateEvent);

            var viewModel = new CompareEbcdicFilesViewModel(eventAggregator, TestHelper.RegionManagerMock,
                TestHelper.FileDialogInteractionMock, TestHelper.CompareEbcdicFilesServiceMock,
                TestHelper.CopybookManagerMock, TestHelper.ExternalProgramServiceMock);

            // Set state to ready perform copybook compare
            viewModel.LegacyEbcdicFilePath = file1;
            viewModel.ModernizedEbcdicFilePath = file2;
            viewModel.CopybookFilePath = copybook;
            viewModel.CurrentState = CompareEbcdicFilesViewModel.States.ReadyToPerformCopybookCompare;

            viewModel.PerformCopybookCompare.Execute();

            finishEvent.Publish(new FinishReadEbcdicFile
            {
                EventType = ReadEbcdicFileEventType.CompareEbcdicFiles,
                CompareEbcdicFileResult = new CompareEbcdicFileResult()
            });

            // Ensure that external program option is shown.
            Assert.AreEqual(CompareEbcdicFilesViewModel.States.FinishedCopybookCompare, viewModel.CurrentState);
        }
    }
}
