using CodeMovement.EbcdicCompare.DataAccess;
using CodeMovement.EbcdicCompare.Models;
using CodeMovement.EbcdicCompare.Models.Constant;
using CodeMovement.EbcdicCompare.Models.Request;
using CodeMovement.EbcdicCompare.Models.Result;
using CodeMovement.EbcdicCompare.Models.ViewModel;
using CodeMovement.EbcdicCompare.Presentation.Event;
using CodeMovement.EbcdicCompare.Presentation.Interaction;
using CodeMovement.EbcdicCompare.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Regions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System;

namespace CodeMovement.EbcdicCompare.Presentation.ViewModel
{
    public partial class CompareEbcdicFilesViewModel : BaseNavigationViewModel
    {
        #region "Attributes"

        private string _legacyEbcdicFilePath;
        private string _modernizedEbcdicFilePath;
        private string _copybookFilePath;
        private bool? _useCopybook;

        private CompareEbcdicFileResult _compareResult;

        private readonly IFileDialogInteraction _fileDialog;
        private readonly ICompareEbcdicFilesService _compareEbcdicFilesService;
        private readonly IExternalProgramService _externalProgramService;
        private readonly ICopybookManager _copybookManager;
        private readonly IConfigurationSettings _configurationSettings;

        private static readonly WindowSize OpenEbcdicFileWindowSize = new WindowSize(1000, 800);

        #endregion

        public CompareEbcdicFilesViewModel(IEventAggregator eventAggregator,
            IRegionManager regionManager,
            IFileDialogInteraction fileDialog,
            ICompareEbcdicFilesService compareEbcdicFilesService,
            ICopybookManager copybookManager,
            IConfigurationSettings configurationSettings,
            IExternalProgramService externalProgramService)
            : base(regionManager, eventAggregator, OpenEbcdicFileWindowSize)
        {
            _fileDialog = fileDialog;
            _compareEbcdicFilesService = compareEbcdicFilesService;
            _externalProgramService = externalProgramService;
            _copybookManager = copybookManager;
            _configurationSettings = configurationSettings;

            ConfigureCommands();

            EventAggregator.GetEvent<FinishReadEbcdicFileEvent>().Subscribe(OnCompareEbcdicFilesComplete,
                ThreadOption.UIThread, false,
                e => e.EventType == ReadEbcdicFileEventType.CompareEbcdicFiles);
        }

        public ICommand SelectLegacyFile { get; private set; }
        public ICommand SelectModernizedFile { get; private set; }
        public DelegateCommand PerformInitialCompare { get; private set; }
        public ICommand SelectUseCopybook { get; private set; }
        public ICommand OpenInExternalProgram { get; private set; }
        public ICommand SelectCopybook { get; private set; }
        public DelegateCommand PerformCopybookCompare { get; private set; }

        public InteractionRequest<INotification> ErrorConfirmationRequest { get; private set; }

        protected CompareEbcdicFileResult CompareEbcdicFileResult
        {
            get { return _compareResult; }
            set
            {
                _compareResult = value;
                OnPropertyChanged("LegacyComparisonResults");
                OnPropertyChanged("ModernizedComparisonResults");
            }
        }

        #region "View Model Properties"

        public string LegacyEbcdicFilePath
        {
            get { return _legacyEbcdicFilePath; }
            set
            {
                _legacyEbcdicFilePath = value;
                OnPropertyChanged("LegacyEbcdicFilePath");
            }
        }

        public string ModernizedEbcdicFilePath
        {
            get { return _modernizedEbcdicFilePath; }
            set
            {
                _modernizedEbcdicFilePath = value;
                OnPropertyChanged("ModernizedEbcdicFilePath");
            }
        }

        public string CopybookFilePath
        {
            get { return _copybookFilePath; }
            set
            {
                _copybookFilePath = value;
                OnPropertyChanged("CopybookFilePath");
            }
        }

        public bool? UseCopybook
        {
            get { return _useCopybook; }
            set
            {
                _useCopybook = value;
                OnPropertyChanged("UseCopybook");
            }
        }

        public bool FilterByRecordDifferences
        {
            get { return _filterByRecordDifferences; }
            set
            {
                _filterByRecordDifferences = value;
                OnPropertyChanged("FilterByRecordDifferences");

                OnShowOnlyRecordDifferences();
            }
        }

        public bool SortAndCompareRecords
        {
            get { return _sortAndCompareRecords; }
            set
            {
                _sortAndCompareRecords = value;
                OnPropertyChanged("SortAndCompareRecords");

                OnSortAndOrderRecords();
            }
        }

        public string LegacyComparisonResults
        {
            get
            {
                return _compareResult == null
                    ? string.Format("Total: {0:N0} | Matched: {1:N0} | Differences: {2:N0} | Extra: {3:N0}", 0, 0, 0, 0)
                    : string.Format("Total: {0:N0} | Matched: {1:N0} | Differences: {2:N0} | Extra: {3:N0}",
                        _compareResult.FirstEbcdicFile.Total,
                        _compareResult.FirstEbcdicFile.Matches,
                        _compareResult.FirstEbcdicFile.Differences,
                        _compareResult.FirstEbcdicFile.Extras);
            }
        }

        public string ModernizedComparisonResults
        {
            get
            {
                return _compareResult == null
                    ? string.Format("Total: {0:N0} | Extra: {1:N0}", 0, 0)
                    : string.Format("Total: {0:N0} | Extra: {1:N0}",
                        _compareResult.SecondEbcdicFile.Total,
                        _compareResult.SecondEbcdicFile.Extras);
            }
        }

        #endregion

        #region "INotifyPropertyChanged"

        protected override void OnPropertyChanged(string propertyName = null)
        {
            if (propertyName == "LegacyEbcdicFilePath" ||
                propertyName == "ModernizedEbcdicFilePath" ||
                propertyName == "CopybookFilePath")
            {
                PerformInitialCompare.RaiseCanExecuteChanged();
                PerformCopybookCompare.RaiseCanExecuteChanged();
            }

            base.OnPropertyChanged(propertyName);
        }

        #endregion

        #region "Event Handlers"

        private void OnSelectLegacyFile()
        {
            LegacyEbcdicFilePath = _fileDialog.OpenFileDialog("Select Legacy EBCDIC file");
        }

        private void OnSelectModernizedFile()
        {
            ModernizedEbcdicFilePath = _fileDialog.OpenFileDialog("Select Modernized EBCDIC file");
        }

        private async Task OnPerformInitialCompare()
        {
            CurrentState = States.PerformingInitialCompare;

            var task = Task.Run(() => _compareEbcdicFilesService.CompareEbcdicByteContents(LegacyEbcdicFilePath,
                ModernizedEbcdicFilePath));

            var compareResults = await task;

            CurrentState = States.FinishedInitialCompare;

            if (!compareResults.Successful)
            {
                ErrorConfirmationRequest.Raise(new Notification
                {
                    Title = "Oops! Something went wrong!",
                    Content = compareResults.Messages[0]
                });
            }

            CurrentState = compareResults.Result.AreIdentical
                ? States.FilesMatch
                : States.FilesDoNotMatch;
        }

        private void OnOpenExternalEditor()
        {
            var openLegacyFileResult = _externalProgramService.RunProgram(_configurationSettings.ExternalEditorPath, 
                string.Format("\"{0}\"", LegacyEbcdicFilePath));
            var openModernizedFileResult = _externalProgramService.RunProgram(_configurationSettings.ExternalEditorPath, 
                string.Format("\"{0}\"", ModernizedEbcdicFilePath));

            if (!openLegacyFileResult.Successful || !openModernizedFileResult.Successful)
            {
                ErrorConfirmationRequest.Raise(new Notification
                {
                    Title = "Oops! Something went wrong!",
                    Content = string.Format("Cannot open EBCDIC files in external editor.\n\nCannot find editor:\n{0}", 
                        _configurationSettings.ExternalEditorPath)
                });
            }

            OnResetView();
        }

        private void OnSelectCopybookFile()
        {
            CopybookFilePath = _fileDialog.OpenFileDialog("Select Copybook XML file");
        }

        private void OnSelectUseCopybook()
        {
            if (!UseCopybook.HasValue)
                return;

            CurrentState = States.ShouldUseCopybook;

            if (UseCopybook.Value)
            {
                if (!FindCopybookForEbcdicFile(LegacyEbcdicFilePath) &&
                    !FindCopybookForEbcdicFile(ModernizedEbcdicFilePath))
                    CurrentState = States.UseCopybook;
            }
            else
            {
                CurrentState = States.DoNotUseCopybook;
            }
        }

        private void OnPerformCopybookCompare()
        {
            CurrentState = States.PerformingCopybookCompare;

            EventAggregator.GetEvent<CompareEbcdicFilesRequestEvent>().Publish(new CompareEbcdicFilesRequest
            {
                CopybookFilePath = CopybookFilePath,
                FirstEbcdicFilePath = LegacyEbcdicFilePath,
                SecondEbcdicFilePath = ModernizedEbcdicFilePath
            });
        }

        private void OnCompareEbcdicFilesComplete(FinishReadEbcdicFile results)
        {
            if (!string.IsNullOrEmpty(results.ErrorMessage))
            {
                ErrorConfirmationRequest.Raise(
                    new Notification { Content = results.ErrorMessage, Title = "Oops! An error has occurred!" });
            }
            else
            {
                CompareEbcdicFileResult = results.CompareEbcdicFileResult;
            }

            CurrentState = States.FinishedCopybookCompare;
        }

        private void OnShowOnlyRecordDifferences()
        {
            var filterCriteria = FilterByRecordDifferences
                ? new List<RecordFlag> { RecordFlag.Different, RecordFlag.Extra }
                : new List<RecordFlag> { RecordFlag.Identical, RecordFlag.Different, RecordFlag.Extra };

            EventAggregator.GetEvent<FilterEbcdicRecordsEvent>().Publish(new FilterEbcdicRecordsRequest
            {
                FilterBy = filterCriteria,
                RegionName = RegionNames.FirstEbcdicFileContentRegion
            });

            EventAggregator.GetEvent<FilterEbcdicRecordsEvent>().Publish(new FilterEbcdicRecordsRequest
            {
                FilterBy = filterCriteria,
                RegionName = RegionNames.SecondEbcdicFileContentRegion
            });
        }

        private void OnSortAndOrderRecords()
        {
            EventAggregator.GetEvent<SortEbcdicRecordsEvent>().Publish(new SortEbcdicRecordsRequest
            {
                SortEbcdicFileRecords = SortAndCompareRecords,
                CompareResult = CompareEbcdicFileResult
            });
        }

        #endregion

        #region "Helper Methods"

        private void ConfigureCommands()
        {
            ErrorConfirmationRequest = new InteractionRequest<INotification>();

            SelectLegacyFile = new DelegateCommand(OnSelectLegacyFile);
            SelectModernizedFile = new DelegateCommand(OnSelectModernizedFile);
            PerformInitialCompare = DelegateCommand.FromAsyncHandler(OnPerformInitialCompare, CanPerformInitialCompare);
            SelectUseCopybook = new DelegateCommand(OnSelectUseCopybook);
            OpenInExternalProgram = new DelegateCommand(OnOpenExternalEditor);
            SelectCopybook = new DelegateCommand(OnSelectCopybookFile);
            PerformCopybookCompare = new DelegateCommand(OnPerformCopybookCompare, CanPerformCopybookCompare);
        }

        private bool FindCopybookForEbcdicFile(string ebcdicFilePath)
        {
            var findCopybookResult = _copybookManager.GetCopybookPathForEbcdicFile(ebcdicFilePath);

            if (findCopybookResult.Result != null)
            {
                CopybookFilePath = findCopybookResult.Result;
                CurrentState = States.ReadyToPerformCopybookCompare;
                return true;
            }

            return false;
        }

        private bool CanPerformInitialCompare()
        {
            var canPerformInitialCompare = !(string.IsNullOrWhiteSpace(LegacyEbcdicFilePath) ||
                                             string.IsNullOrWhiteSpace(ModernizedEbcdicFilePath));

            if (canPerformInitialCompare)
                CurrentState = States.ReadyToPerformInitialCompare;

            return canPerformInitialCompare;
        }

        private bool CanPerformCopybookCompare()
        {
            var canPerformCopybookCompare = CanPerformInitialCompare() && !string.IsNullOrEmpty(CopybookFilePath);

            if (canPerformCopybookCompare)
                CurrentState = States.ReadyToPerformCopybookCompare;

            return canPerformCopybookCompare;
        }

        #endregion

        #region "BaseNavigationViewModel"

        protected override void OnResetView()
        {
            CurrentState = States.Initial;

            EventAggregator.GetEvent<UpdateEbcdicFileGridEvent>().Publish(new UpdateEbcdicFileGridResult
            {
                Region = RegionNames.FirstEbcdicFileContentRegion,
                AllEbcdicFileRecordModels = new ObservableCollection<EbcdicFileRecordModel>(),
                VisibleEbcdicFileRecords = new ObservableCollection<EbcdicFileRecordModel>()
            });

            EventAggregator.GetEvent<UpdateEbcdicFileGridEvent>().Publish(new UpdateEbcdicFileGridResult
            {
                Region = RegionNames.SecondEbcdicFileContentRegion,
                AllEbcdicFileRecordModels = new ObservableCollection<EbcdicFileRecordModel>(),
                VisibleEbcdicFileRecords = new ObservableCollection<EbcdicFileRecordModel>()
            });
        }

        #endregion
    }
}
