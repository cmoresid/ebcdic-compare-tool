using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using CodeMovement.EbcdicCompare.Presentation.ViewModel;
using CodeMovement.EbcdicCompare.Models.Constant;
using Prism.Events;
using CodeMovement.EbcdicCompare.Services;
using CodeMovement.EbcdicCompare.Tests;
using CodeMovement.EbcdicCompare.Presentation.Controller;
using CodeMovement.EbcdicCompare.Models.Request;
using System;
using System.IO;
using CodeMovement.EbcdicCompare.DataAccess;
using CodeMovement.EbcdicCompare.Models.Result;
using System.Collections.Generic;

namespace CodeMovement.EbcdicCompare.UnitTests.Controllers
{
    [TestClass]
    public class EbcdicFileContentRegionControllerTests
    {
        private ICompareEbcdicFilesService CompareEbcdicFilesService
        {
            get { return new CompareEbcdicFilesService(new EbcdicReaderService(), new FileOperation()); }
        }

        private ICompareEbcdicFilesService CompareEbcdicFilesServiceMock
        {
            get { return MockRepository.GenerateMock<ICompareEbcdicFilesService>(); }
        }

        [TestMethod]
        public void Does_EbcdicFileContentRegionController_Subscribe_To_Events()
        {
            var viewEvent = new ViewEbcdicFileRequestEventMock();
            var compareEvent = new CompareEbcdicFilesRequestEventMock();
            var sortEvent = new SortEbcdicRecordsEventMock();

            var eventAggregator = TestHelper.CreateEventAggregator(viewFileEvent: viewEvent, compareEvent: compareEvent, sortEvent: sortEvent);

            var controller = new EbcdicFileContentRegionController(eventAggregator, CompareEbcdicFilesService);

            Assert.IsNotNull(viewEvent.Callback);
            Assert.IsNotNull(compareEvent.Callback);
            Assert.IsNotNull(sortEvent.Callback);
        }

        [TestMethod]
        public void Does_EbcdicFileContentRegionController_View_Ebcdic_File_Successfully()
        {
            Action<FinishReadEbcdicFile> finishEventAssertions = (result) =>
            {
                Assert.AreEqual(ReadEbcdicFileEventType.ViewEbcdicFile, result.EventType);
                Assert.IsTrue(string.IsNullOrWhiteSpace(result.ErrorMessage));
            };

            var viewEvent = new ViewEbcdicFileRequestEventMock();
            var finishEvent = new FinishReadEbcdicFileEventMock();
            finishEvent.Subscribe(finishEventAssertions, ThreadOption.UIThread, false, null);

            var compareEvent = new CompareEbcdicFilesRequestEventMock();
            var updateEvent = new UpdateEbcdicFileGridEventMock();
            var eventAggregator = TestHelper.CreateEventAggregator(viewFileEvent: viewEvent, 
                updateEvent: updateEvent,
                finishReadEvent: finishEvent);
            var gridViewModel = new EbcdicFileGridViewModel(eventAggregator);
            gridViewModel.RegionName = RegionNames.ViewEbcdicFileContentRegion;

            var controller = new EbcdicFileContentRegionController(eventAggregator, CompareEbcdicFilesService);

            viewEvent.Publish(new ViewEbcdicFileRequest
            {
                EbcdicFilePath = Path.Combine(TestHelper.EbcdicFiles, "Person.txt"),
                CopybookFilePath = Path.Combine(TestHelper.Copybooks, "Person.fileformat")
            });

            Assert.AreEqual(10, gridViewModel.VisibleEbcdicFileRecords.Count);
            Assert.AreEqual(10, gridViewModel.AllEbcdicFileRecordModels.Count);
        }

        public void Does_EbcdicFileContentRegionController_View_Ebcdic_File_Fail_Gracefully()
        {
            Action<FinishReadEbcdicFile> finishEventAssertions = (result) =>
            {
                Assert.AreEqual(ReadEbcdicFileEventType.ViewEbcdicFile, result.EventType);
                Assert.IsFalse(string.IsNullOrWhiteSpace(result.ErrorMessage));
            };

            var viewEvent = new ViewEbcdicFileRequestEventMock();
            var finishEvent = new FinishReadEbcdicFileEventMock();
            finishEvent.Subscribe(finishEventAssertions, ThreadOption.UIThread, false, null);

            var compareEvent = new CompareEbcdicFilesRequestEventMock();
            var updateEvent = new UpdateEbcdicFileGridEventMock();
            var eventAggregator = TestHelper.CreateEventAggregator(viewFileEvent: viewEvent,
                compareEvent: compareEvent,
                updateEvent: updateEvent,
                finishReadEvent: finishEvent);
            var gridViewModel = new EbcdicFileGridViewModel(eventAggregator);
            gridViewModel.RegionName = RegionNames.ViewEbcdicFileContentRegion;

            var compareServiceMock = CompareEbcdicFilesServiceMock;
            compareServiceMock.Expect(m => m.Compare(Arg<CompareEbcdicFilesRequest>.Is.Anything)).Return(new OperationResult<CompareEbcdicFileResult>
            {
                Messages = new List<string>() { "An error message" }
            });

            var controller = new EbcdicFileContentRegionController(eventAggregator, compareServiceMock);

            viewEvent.Publish(new ViewEbcdicFileRequest
            {
                EbcdicFilePath = Path.Combine(TestHelper.EbcdicFiles, "Person.txt"),
                CopybookFilePath = Path.Combine(TestHelper.Copybooks, "Person.fileformat")
            });

            Assert.AreEqual(0, gridViewModel.VisibleEbcdicFileRecords.Count);
            Assert.AreEqual(0, gridViewModel.AllEbcdicFileRecordModels.Count);
        }

        [TestMethod]
        public void Does_EbcdicFileContentRegionController_Compare_Ebcdic_Files_Successfully()
        {
            Action<FinishReadEbcdicFile> finishEventAssertions = (result) =>
            {
                Assert.AreEqual(ReadEbcdicFileEventType.CompareEbcdicFiles, result.EventType);
                Assert.IsTrue(string.IsNullOrWhiteSpace(result.ErrorMessage));
            };

            var finishEvent = new FinishReadEbcdicFileEventMock();
            finishEvent.Subscribe(finishEventAssertions, ThreadOption.UIThread, false, null);

            var compareEvent = new CompareEbcdicFilesRequestEventMock();
            var updateEvent = new UpdateEbcdicFileGridEventMock();
            var eventAggregator = TestHelper.CreateEventAggregator(compareEvent: compareEvent,
                updateEvent: updateEvent,
                finishReadEvent: finishEvent);

            var firstGridView = new EbcdicFileGridViewModel(eventAggregator);
            firstGridView.RegionName = RegionNames.FirstEbcdicFileContentRegion;

            var secondGridView = new EbcdicFileGridViewModel(eventAggregator);
            secondGridView.RegionName = RegionNames.SecondEbcdicFileContentRegion;

            var controller = new EbcdicFileContentRegionController(eventAggregator, CompareEbcdicFilesService);

            compareEvent.Publish(new CompareEbcdicFilesRequest
            {
                FirstEbcdicFilePath = Path.Combine(TestHelper.EbcdicFiles, "Person.txt"),
                SecondEbcdicFilePath = Path.Combine(TestHelper.EbcdicFiles, "Person1_B.txt"),
                CopybookFilePath = Path.Combine(TestHelper.Copybooks, "Person.fileformat")
            });

            Assert.AreEqual(10, firstGridView.VisibleEbcdicFileRecords.Count);
            Assert.AreEqual(2, secondGridView.AllEbcdicFileRecordModels.Count);
        }

        [TestMethod]
        public void Does_EbcdicFileContentRegionController_Sort_Records_Successfully()
        {
            CompareEbcdicFileResult compareResults = null;
            bool firstComparePerformed = false;

            Action<FinishReadEbcdicFile> finishEventAssertions = (result) =>
            {
                if (!firstComparePerformed)
                {
                    compareResults = result.CompareEbcdicFileResult;
                    firstComparePerformed = true;
                }
                else
                {
                    Assert.IsNotNull(result.CompareEbcdicFileResult);
                }
            };

            var finishEvent = new FinishReadEbcdicFileEventMock();
            finishEvent.Subscribe(finishEventAssertions, ThreadOption.UIThread, false, null);

            var compareEvent = new CompareEbcdicFilesRequestEventMock();
            var sortEvent = new SortEbcdicRecordsEventMock();
            var updateEvent = new UpdateEbcdicFileGridEventMock();
            var eventAggregator = TestHelper.CreateEventAggregator(compareEvent: compareEvent,
                updateEvent: updateEvent,
                finishReadEvent: finishEvent);

            var firstGridView = new EbcdicFileGridViewModel(eventAggregator);
            firstGridView.RegionName = RegionNames.FirstEbcdicFileContentRegion;

            var secondGridView = new EbcdicFileGridViewModel(eventAggregator);
            secondGridView.RegionName = RegionNames.SecondEbcdicFileContentRegion;

            var controller = new EbcdicFileContentRegionController(eventAggregator, CompareEbcdicFilesService);

            compareEvent.Publish(new CompareEbcdicFilesRequest
            {
                FirstEbcdicFilePath = Path.Combine(TestHelper.EbcdicFiles, "SortPersonA.txt"),
                SecondEbcdicFilePath = Path.Combine(TestHelper.EbcdicFiles, "SortPersonB_B.txt"),
                CopybookFilePath = Path.Combine(TestHelper.Copybooks, "Person.fileformat")
            });

            Assert.IsNotNull(compareResults);

            sortEvent.Publish(new SortEbcdicRecordsRequest
            {
                CompareResult = compareResults,
                SortEbcdicFileRecords = true
            });
        }
    }
}
