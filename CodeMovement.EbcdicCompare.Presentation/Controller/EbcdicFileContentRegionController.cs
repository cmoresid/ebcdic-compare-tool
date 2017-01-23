using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Regions;
using CodeMovement.EbcdicCompare.Models;
using CodeMovement.EbcdicCompare.Models.Constant;
using CodeMovement.EbcdicCompare.Models.Request;
using CodeMovement.EbcdicCompare.Models.Result;
using CodeMovement.EbcdicCompare.Models.ViewModel;
using CodeMovement.EbcdicCompare.Services;
using CodeMovement.EbcdicCompare.Presentation.Event;
using CodeMovement.EbcdicCompare.Presentation.ViewModel;
using CodeMovement.EbcdicCompare.Presentation.View;

namespace CodeMovement.EbcdicCompare.Presentation.Controller
{
    public class EbcdicFileContentRegionController
    {
        private readonly IUnityContainer _container;
        private readonly IRegionManager _regionManager;
        private readonly ICompareEbcdicFilesService _compareEbcdicFilesService;
        private readonly IEventAggregator _eventAggregator;
        private readonly SynchronizationContext _syncContext;

        public EbcdicFileContentRegionController(IUnityContainer container,
            IRegionManager regionManager,
            ICompareEbcdicFilesService compareEbcdicFilesService,
            IEventAggregator eventAggregator)
        {
            _container = container;
            _regionManager = regionManager;
            _compareEbcdicFilesService = compareEbcdicFilesService;
            _eventAggregator = eventAggregator;
            _syncContext = SynchronizationContext.Current; /* Stores reference to ui thread */

            _eventAggregator.GetEvent<ViewEbcdicFileRequestEvent>().Subscribe(ViewEbcdicFileHandler,
                ThreadOption.BackgroundThread, true);

            _eventAggregator.GetEvent<CompareEbcdicFilesRequestEvent>().Subscribe(CompareEbcdicFilesHander,
                ThreadOption.BackgroundThread, true);

            _eventAggregator.GetEvent<ClearEbcdicFileGridEvent>().Subscribe(ClearEbcdicFileGrid,
                ThreadOption.UIThread, true);

            _eventAggregator.GetEvent<FilterEbcdicRecordsEvent>().Subscribe(FilterByEbcdicFileGrid, 
                ThreadOption.UIThread, true);
        }

        #region "Event Handlers"

        private void FilterByEbcdicFileGrid(FilterEbcdicRecordsRequest request)
        {
            switch (request.Target)
            {
                case EbcdicRecordGrid.Both:
                    FilterRecordsForRegionDataGrid(RegionNames.FirstEbcdicFileContentRegion, request.FilterBy);
                    FilterRecordsForRegionDataGrid(RegionNames.SecondEbcdicFileContentRegion, request.FilterBy);
                    break;
                case EbcdicRecordGrid.LegacyGrid:
                    FilterRecordsForRegionDataGrid(RegionNames.FirstEbcdicFileContentRegion, request.FilterBy);
                    break;
                case EbcdicRecordGrid.ModernizedGrid:
                    FilterRecordsForRegionDataGrid(RegionNames.SecondEbcdicFileContentRegion, request.FilterBy);
                    break;
                default:
                    break;
            }
        }

        private void ClearEbcdicFileGrid(ClearEbcdicFileGridRequest request)
        {
            if (request.EventType == ReadEbcdicFileEventType.ViewEbcdicFile)
            {
                SetEbcdicFileRecordsForRegionDataGrid(RegionNames.ViewEbcdicFileContentRegion, 
                    new ObservableCollection<EbcdicFileRecordModel>());
            }

            if (request.EventType == ReadEbcdicFileEventType.CompareEbcdicFiles)
            {
                SetEbcdicFileRecordsForRegionDataGrid(RegionNames.FirstEbcdicFileContentRegion,
                    new ObservableCollection<EbcdicFileRecordModel>());
                SetEbcdicFileRecordsForRegionDataGrid(RegionNames.SecondEbcdicFileContentRegion,
                    new ObservableCollection<EbcdicFileRecordModel>());
            }
        }

        private void ViewEbcdicFileHandler(ViewEbcdicFileRequest request)
        {
            var finishReadEbcdicFile = new FinishReadEbcdicFile
            {
                EventType = ReadEbcdicFileEventType.ViewEbcdicFile
            };

            // Clear the existing grid results first
            SetEbcdicFileRecordsForRegionDataGrid(RegionNames.ViewEbcdicFileContentRegion,
                        new ObservableCollection<EbcdicFileRecordModel>());

            // Read in the EBCDIC file
            var results = _compareEbcdicFilesService.Compare(new CompareEbcdicFilesRequest
            {
                FirstEbcdicFilePath = request.EbcdicFilePath,
                SecondEbcdicFilePath = null,
                CopybookFilePath = request.CopybookFilePath,
                Encoding = Encoding.Ascii
            });

            if (results.Messages.Count == 0)
            {
                // Update grid view with results
                SetEbcdicFileRecordsForRegionDataGrid(RegionNames.ViewEbcdicFileContentRegion,
                    results.Result.FirstEbcdicFile.EbcdicFileRecords);
            }
            else
            {
                finishReadEbcdicFile.ErrorMessage = results.Messages[0];
            }

            // Notify the view of any errors that occurred during the reading of the
            // EBCDIC file
            _eventAggregator.GetEvent<FinishReadEbcdicFileEvent>().Publish(finishReadEbcdicFile);
        }

        private void CompareEbcdicFilesHander(CompareEbcdicFilesRequest request)
        {
            var finishedReadingEvent = _eventAggregator.GetEvent<FinishReadEbcdicFileEvent>();
            var finishReadEbcdicFile = new FinishReadEbcdicFile
            {
                EventType = ReadEbcdicFileEventType.CompareEbcdicFiles
            };

            SetEbcdicFileRecordsForRegionDataGrid(RegionNames.FirstEbcdicFileContentRegion,
                new ObservableCollection<EbcdicFileRecordModel>());
            SetEbcdicFileRecordsForRegionDataGrid(RegionNames.SecondEbcdicFileContentRegion,
                new ObservableCollection<EbcdicFileRecordModel>());

            var comparisonResults = _compareEbcdicFilesService.Compare(request);

            if (comparisonResults.Messages.Count > 0)
            {
                finishReadEbcdicFile.ErrorMessage = comparisonResults.Messages[0];
                finishedReadingEvent.Publish(new FinishReadEbcdicFile
                {
                    ErrorMessage = comparisonResults.Messages[0]
                });
            }

            SetEbcdicFileRecordsForRegionDataGrid(RegionNames.FirstEbcdicFileContentRegion,
                comparisonResults.Result.FirstEbcdicFile.EbcdicFileRecords);
            SetEbcdicFileRecordsForRegionDataGrid(RegionNames.SecondEbcdicFileContentRegion,
                comparisonResults.Result.SecondEbcdicFile.EbcdicFileRecords);

            finishReadEbcdicFile.CompareEbcdicFileResult = comparisonResults.Result;
            finishedReadingEvent.Publish(finishReadEbcdicFile);
        }

        #endregion

        #region "Helper Methods"

        private void FilterRecordsForRegionDataGrid(string regionName,
            List<RecordFlag> filterCriteria)
        {
            // Need to update the view on the ui thread; otherwise bad things
            // will happen...
            _syncContext.Post(u =>
            {
                var viewEbcdicFileContentRegion = _regionManager.Regions[regionName];
                if (viewEbcdicFileContentRegion == null)
                    return;

                var view = viewEbcdicFileContentRegion.Views.FirstOrDefault() as EbcdicFileGridView;
                if (view == null)
                {
                    view = _container.Resolve<EbcdicFileGridView>();
                    viewEbcdicFileContentRegion.Add(view);
                }

                var viewModel = view.DataContext as EbcdicFileGridViewModel;
                if (viewModel != null)
                {
                    var filteredRecords =
                        viewModel.AllEbcdicFileRecordModels.Where(record => filterCriteria.Contains(record.Flag));

                    viewModel.VisibleEbcdicFileRecords = new ObservableCollection<EbcdicFileRecordModel>(filteredRecords);
                }
            }, null);
        }

        private void SetEbcdicFileRecordsForRegionDataGrid(string regionName,
            ObservableCollection<EbcdicFileRecordModel> records)
        {
            // Need to update the view on the ui thread; otherwise bad things
            // will happen...
            _syncContext.Post(u =>
            {
                var viewEbcdicFileContentRegion = _regionManager.Regions[regionName];
                if (viewEbcdicFileContentRegion == null)
                    return;

                var view = viewEbcdicFileContentRegion.Views.FirstOrDefault() as EbcdicFileGridView;
                if (view == null)
                {
                    view = _container.Resolve<EbcdicFileGridView>();
                    viewEbcdicFileContentRegion.Add(view);
                }

                var viewModel = view.DataContext as EbcdicFileGridViewModel;
                if (viewModel != null)
                {
                    viewModel.AllEbcdicFileRecordModels = records;
                    viewModel.VisibleEbcdicFileRecords = records;
                }
            }, null);
        }

        #endregion
    }
}