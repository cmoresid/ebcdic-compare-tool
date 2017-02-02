using CodeMovement.EbcdicCompare.Presentation.Event;
using CodeMovement.EbcdicCompare.Presentation.Interaction;
using CodeMovement.EbcdicCompare.Services;
using CodeMovement.EbcdicCompare.Tests;
using Prism.Events;
using Prism.Regions;
using Rhino.Mocks;

namespace CodeMovement.EbcdicCompare.UnitTests
{
    public static class TestHelper
    {
        public const string TestData = @".\TestData";
        public const string Copybooks = TestData + @"\Copybooks";
        public const string EbcdicFiles = TestData + @"\EbcdicFiles";

        public static IEventAggregator CreateEventAggregator(FinishReadEbcdicFileEventMock finishReadEvent = null,
            ViewEbcdicFileRequestEventMock viewFileEvent = null,
            CompareEbcdicFilesRequestEventMock compareEvent = null,
            FilterEbcdicRecordsEventMock filterEvent = null,
            UpdateEbcdicFileGridEventMock updateEvent = null)
        {
            var eventAggregatorMock = MockRepository.GenerateMock<IEventAggregator>();

            finishReadEvent = finishReadEvent ?? new FinishReadEbcdicFileEventMock();
            viewFileEvent = viewFileEvent ?? new ViewEbcdicFileRequestEventMock();
            compareEvent = compareEvent ?? new CompareEbcdicFilesRequestEventMock();
            filterEvent = filterEvent ?? new FilterEbcdicRecordsEventMock();
            updateEvent = updateEvent ?? new UpdateEbcdicFileGridEventMock();

            eventAggregatorMock.Expect(m => m.GetEvent<FinishReadEbcdicFileEvent>())
                    .Return(finishReadEvent).Repeat.Any();

            eventAggregatorMock.Expect(m => m.GetEvent<ViewEbcdicFileRequestEvent>())
                .Return(viewFileEvent).Repeat.Any();

            eventAggregatorMock.Expect(m => m.GetEvent<CompareEbcdicFilesRequestEvent>())
               .Return(compareEvent).Repeat.Any();

            eventAggregatorMock.Expect(m => m.GetEvent<FilterEbcdicRecordsEvent>())
               .Return(filterEvent).Repeat.Any();

            eventAggregatorMock.Expect(m => m.GetEvent<UpdateEbcdicFileGridEvent>())
               .Return(updateEvent).Repeat.Any();

            return eventAggregatorMock;
        }

        public static IRegionManager RegionManagerMock
        {
            get { return MockRepository.GenerateMock<IRegionManager>(); }
        }

        public static ICopybookManager CopybookManagerMock
        {
            get { return MockRepository.GenerateMock<ICopybookManager>(); }
        }

        public static IFileDialogInteraction FileDialogInteractionMock
        {
            get { return MockRepository.GenerateMock<IFileDialogInteraction>(); }
        }

        public static IFileOperationsManager FileOperationsManagerMock
        {
            get { return MockRepository.GenerateMock<IFileOperationsManager>(); }
        }

        public static IExternalProgramService ExternalProgramServiceMock
        {
            get { return MockRepository.GenerateMock<IExternalProgramService>(); }
        }

        public static ICompareEbcdicFilesService CompareEbcdicFilesServiceMock
        {
            get { return MockRepository.GenerateMock<ICompareEbcdicFilesService>(); }
        }
    }
}
