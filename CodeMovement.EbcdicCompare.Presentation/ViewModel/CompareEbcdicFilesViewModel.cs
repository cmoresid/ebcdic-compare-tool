using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Regions;
using CodeMovement.EbcdicCompare.Models;
using CodeMovement.EbcdicCompare.Models.Constant;
using CodeMovement.EbcdicCompare.Models.Request;
using CodeMovement.EbcdicCompare.Models.Result;
using CodeMovement.EbcdicCompare.Services;
using CodeMovement.EbcdicCompare.Presentation.Event;
using CodeMovement.EbcdicCompare.Presentation.Interaction;

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

        private static readonly WindowSize OpenEbcdicFileWindowSize = new WindowSize(1000, 700);

        #endregion

        public CompareEbcdicFilesViewModel(IEventAggregator eventAggregator,
            IRegionManager regionManager,
            IFileDialogInteraction fileDialog,
            ICompareEbcdicFilesService compareEbcdicFilesService,
            ICopybookManager copybookManager,
            IExternalProgramService externalProgramService)
            : base(regionManager, eventAggregator, OpenEbcdicFileWindowSize)
        {
            _fileDialog = fileDialog;
            _compareEbcdicFilesService = compareEbcdicFilesService;
            _externalProgramService = externalProgramService;
            _copybookManager = copybookManager;

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
            set { SetProperty(ref _legacyEbcdicFilePath, value); }
        }

        public string ModernizedEbcdicFilePath
        {
            get { return _modernizedEbcdicFilePath; }
            set { SetProperty(ref _modernizedEbcdicFilePath, value); }
        }

        public string CopybookFilePath
        {
            get { return _copybookFilePath; }
            set { SetProperty(ref _copybookFilePath, value); }
        }

        public bool? UseCopybook
        {
            get { return _useCopybook; }
            set { SetProperty(ref _useCopybook, value); }
        }

        public bool FilterByRecordDifferences
        {
            get { return _filterByRecordDifferences; }
            set
            {
                SetProperty(ref _filterByRecordDifferences, value);
                OnShowOnlyRecordDifferences();
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
            CurrentState = States.PerformingInitialCompare; ;

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
            _externalProgramService.RunProgram(ExternalEditor, string.Format("\"{0}\"", LegacyEbcdicFilePath));
            _externalProgramService.RunProgram(ExternalEditor, string.Format("\"{0}\"", ModernizedEbcdicFilePath));

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
                Encoding = Encoding.Ascii,
                FirstEbcdicFilePath = LegacyEbcdicFilePath,
                SecondEbcdicFilePath = ModernizedEbcdicFilePath
            });
        }

        private void OnCompareEbcdicFilesComplete(FinishReadEbcdicFile results)
        {
            CurrentState = States.FinishedCopybookCompare;

            if (!string.IsNullOrEmpty(results.ErrorMessage))
            {
                ErrorConfirmationRequest.Raise(
                    new Notification { Content = results.ErrorMessage, Title = "Oops! An error has occurred!" });
            }
            else
            {
                CompareEbcdicFileResult = results.CompareEbcdicFileResult;
            }
        }

        private void OnShowOnlyRecordDifferences()
        {
            var filterCriteria = FilterByRecordDifferences
                ? new List<RecordFlag> { RecordFlag.Different, RecordFlag.Extra }
                : new List<RecordFlag> { RecordFlag.Identical, RecordFlag.Different, RecordFlag.Extra };

            EventAggregator.GetEvent<FilterEbcdicRecordsEvent>().Publish(new FilterEbcdicRecordsRequest
            {
                FilterBy = filterCriteria,
                Target = EbcdicRecordGrid.Both
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

        private static string ExternalEditor
        {
            get { return ConfigurationManager.AppSettings["ExternalEditor"]; }
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

            EventAggregator.GetEvent<ClearEbcdicFileGridEvent>().Publish(new ClearEbcdicFileGridRequest
            {
                EventType = ReadEbcdicFileEventType.CompareEbcdicFiles
            });
        }

        #endregion
    }
}
