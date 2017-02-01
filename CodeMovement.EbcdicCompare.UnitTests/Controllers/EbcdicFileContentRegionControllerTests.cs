using Microsoft.VisualStudio.TestTools.UnitTesting;
using CodeMovement.EbcdicCompare.Presentation.View;
using Microsoft.Practices.Unity;
using Rhino.Mocks;
using Prism.Regions;
using CodeMovement.EbcdicCompare.Presentation.ViewModel;
using CodeMovement.EbcdicCompare.Models.Constant;
using Prism.Events;
using CodeMovement.EbcdicCompare.Services;
using CodeMovement.EbcdicCompare.Tests;
using CodeMovement.EbcdicCompare.Presentation.Event;
using CodeMovement.EbcdicCompare.Presentation.Controller;
using CodeMovement.EbcdicCompare.Models.Request;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace CodeMovement.EbcdicCompare.UnitTests.Controllers
{
    [TestClass]
    public class EbcdicFileContentRegionControllerTests
    {
        private const string TestData = @".\TestData";
        private const string Copybooks = TestData + @"\Copybooks";
        private const string EbcdicFiles = TestData + @"\EbcdicFiles";

        public IEventAggregator CreateEventAggregator(FinishReadEbcdicFileEventMock finishReadEvent = null,
            ViewEbcdicFileRequestEventMock viewFileEvent = null,
            CompareEbcdicFilesRequestEvent compareEvent = null,
            FilterEbcdicRecordsEvent filterEvent = null)
        {
            var eventAggregatorMock = MockRepository.GenerateMock<IEventAggregator>();

            finishReadEvent = finishReadEvent ?? new FinishReadEbcdicFileEventMock();
            viewFileEvent = viewFileEvent ?? new ViewEbcdicFileRequestEventMock();
            compareEvent = compareEvent ?? new CompareEbcdicFilesRequestEventMock();
            filterEvent = filterEvent ?? new FilterEbcdicRecordsEventMock();

            eventAggregatorMock.Expect(m => m.GetEvent<FinishReadEbcdicFileEvent>())
                    .Return(finishReadEvent).Repeat.Any();

            eventAggregatorMock.Expect(m => m.GetEvent<ViewEbcdicFileRequestEvent>())
                .Return(viewFileEvent).Repeat.Any();

            eventAggregatorMock.Expect(m => m.GetEvent<CompareEbcdicFilesRequestEvent>())
               .Return(compareEvent).Repeat.Any();

            eventAggregatorMock.Expect(m => m.GetEvent<FilterEbcdicRecordsEvent>())
               .Return(filterEvent).Repeat.Any();

            return eventAggregatorMock;
        }

        [TestMethod]
        public void TestMethod1()
        {
            var viewEvent = new ViewEbcdicFileRequestEventMock();
            var eventAggregator = CreateEventAggregator(viewFileEvent: viewEvent);

            //var controller = new EbcdicFileContentRegionController(ContainerMock, 
            //    regionManager, CompareEbcdicFilesServiceMock, eventAggregator);

            //viewEvent.Publish(new ViewEbcdicFileRequest
            //{
            //    EbcdicFilePath = Path.Combine(EbcdicFiles, "Person1_A.txt"),
            //    CopybookFilePath = Path.Combine(Copybooks, "Person.fileformat")
            //});


        }
    }
}
