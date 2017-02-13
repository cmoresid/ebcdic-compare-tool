using System.Collections.ObjectModel;
using Prism.Events;
using CodeMovement.EbcdicCompare.Models.Constant;
using CodeMovement.EbcdicCompare.Models.Request;
using CodeMovement.EbcdicCompare.Models.Result;
using CodeMovement.EbcdicCompare.Models.ViewModel;
using CodeMovement.EbcdicCompare.Services;
using CodeMovement.EbcdicCompare.Presentation.Event;

namespace CodeMovement.EbcdicCompare.Presentation.Controller
{
    public class EbcdicFileContentRegionController
    {
        private readonly ICompareEbcdicFilesService _compareEbcdicFilesService;
        private readonly IEventAggregator _eventAggregator;
        private readonly UpdateEbcdicFileGridEvent _updateGridEvent;
        private readonly FinishReadEbcdicFileEvent _finishReadEbcdicFileEvent;

        public EbcdicFileContentRegionController(
            IEventAggregator eventAggregator,
            ICompareEbcdicFilesService compareEbcdicFilesService)
        {
            _compareEbcdicFilesService = compareEbcdicFilesService;
            _eventAggregator = eventAggregator;

            _updateGridEvent = _eventAggregator.GetEvent<UpdateEbcdicFileGridEvent>();
            _finishReadEbcdicFileEvent = eventAggregator.GetEvent<FinishReadEbcdicFileEvent>();

            _eventAggregator.GetEvent<ViewEbcdicFileRequestEvent>().Subscribe(OnViewEbcdicFile,
                ThreadOption.BackgroundThread, true, null);

            _eventAggregator.GetEvent<CompareEbcdicFilesRequestEvent>().Subscribe(OnCompareEbcdicFiles,
                ThreadOption.BackgroundThread, true);

            _eventAggregator.GetEvent<SortEbcdicRecordsEvent>().Subscribe(OnSortAndCompareRecords,
                ThreadOption.BackgroundThread, true, null);
        }

        #region "Event Handlers"

        private void OnSortAndCompareRecords(SortEbcdicRecordsRequest sortRequest)
        {
            var finishReadEbcdicFile = new FinishReadEbcdicFile
            {
                EventType = ReadEbcdicFileEventType.CompareEbcdicFiles
            };

            var comparisonResults = _compareEbcdicFilesService.SortCompareEbcdicResults(
                sortRequest.CompareResult, sortRequest.SortEbcdicFileRecords);

            UpdateEbcdicFileGrid(RegionNames.FirstEbcdicFileContentRegion,
                comparisonResults.Result.FirstEbcdicFile.EbcdicFileRecords);
            UpdateEbcdicFileGrid(RegionNames.SecondEbcdicFileContentRegion,
                comparisonResults.Result.SecondEbcdicFile.EbcdicFileRecords);

            finishReadEbcdicFile.CompareEbcdicFileResult = comparisonResults.Result;
            _finishReadEbcdicFileEvent.Publish(finishReadEbcdicFile);
        }

        private void OnViewEbcdicFile(ViewEbcdicFileRequest request)
        {
            var finishReadEbcdicFile = new FinishReadEbcdicFile
            {
                EventType = ReadEbcdicFileEventType.ViewEbcdicFile
            };

            UpdateEbcdicFileGrid(RegionNames.ViewEbcdicFileContentRegion,
                new ObservableCollection<EbcdicFileRecordModel>());

            // Read in the EBCDIC file
            var results = _compareEbcdicFilesService.Compare(new CompareEbcdicFilesRequest
            {
                FirstEbcdicFilePath = request.EbcdicFilePath,
                SecondEbcdicFilePath = null,
                CopybookFilePath = request.CopybookFilePath
            });

            if (results.Messages.Count == 0)
            {
                // Update grid view with results
                UpdateEbcdicFileGrid(RegionNames.ViewEbcdicFileContentRegion, 
                    results.Result.FirstEbcdicFile.EbcdicFileRecords);
            }
            else
            {
                finishReadEbcdicFile.ErrorMessage = results.Messages[0];
            }

            // Notify the view of any errors that occurred during the reading of the
            // EBCDIC file
            _finishReadEbcdicFileEvent.Publish(finishReadEbcdicFile);
        }

        private void OnCompareEbcdicFiles(CompareEbcdicFilesRequest request)
        {
            var finishReadEbcdicFile = new FinishReadEbcdicFile
            {
                EventType = ReadEbcdicFileEventType.CompareEbcdicFiles
            };

            UpdateEbcdicFileGrid(RegionNames.FirstEbcdicFileContentRegion,
                new ObservableCollection<EbcdicFileRecordModel>());
            UpdateEbcdicFileGrid(RegionNames.SecondEbcdicFileContentRegion,
                new ObservableCollection<EbcdicFileRecordModel>());

            var comparisonResults = _compareEbcdicFilesService.Compare(request);

            if (comparisonResults.Messages.Count > 0)
                finishReadEbcdicFile.ErrorMessage = comparisonResults.Messages[0];

            UpdateEbcdicFileGrid(RegionNames.FirstEbcdicFileContentRegion,
                comparisonResults.Result.FirstEbcdicFile.EbcdicFileRecords);
            UpdateEbcdicFileGrid(RegionNames.SecondEbcdicFileContentRegion,
                comparisonResults.Result.SecondEbcdicFile.EbcdicFileRecords);

            finishReadEbcdicFile.CompareEbcdicFileResult = comparisonResults.Result;
            _finishReadEbcdicFileEvent.Publish(finishReadEbcdicFile);
        }

        #endregion

        #region "Helper Methods"

        private void UpdateEbcdicFileGrid(string regionName, ObservableCollection<EbcdicFileRecordModel> allModels,
            ObservableCollection<EbcdicFileRecordModel> visibleModels = null)
        {
            visibleModels = visibleModels ?? allModels;

            _updateGridEvent.Publish(new UpdateEbcdicFileGridResult
            {
                Region = regionName,
                AllEbcdicFileRecordModels = allModels,
                VisibleEbcdicFileRecords = visibleModels
            });
        }

        #endregion
    }
}