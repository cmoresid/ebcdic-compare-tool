using System.Collections.Generic;
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
using System.Collections.ObjectModel;
using CodeMovement.EbcdicCompare.Models.ViewModel;

namespace CodeMovement.EbcdicCompare.Presentation.ViewModel
{
    public class OpenEbcdicFileViewModel : BaseNavigationViewModel
    {
        #region "Attributes"

        private string _ebcdicFilePath;
        private string _copybookFilePath;
        private bool _isLoadingEbcdicFile;

        private readonly IFileDialogInteraction _fileDialog;
        private readonly ICopybookManager _copybookManager;

        private static readonly WindowSize OpenEbcdicFileWindowSize = new WindowSize(600, 580);

        #endregion

        public OpenEbcdicFileViewModel(IEventAggregator eventAggregator,
            IRegionManager regionManager,
            ICopybookManager copybookManager,
            IFileDialogInteraction fileDialog)
            : base(regionManager, eventAggregator, OpenEbcdicFileWindowSize)
        {
            _fileDialog = fileDialog;
            _copybookManager = copybookManager;

            ErrorConfirmationRequest = new InteractionRequest<INotification>();

            ReadEbcdicFile = new DelegateCommand(ViewEbcdicFile, CanViewEbcdicFile);

            SelectEbcdicFile =
                new DelegateCommand(() => { EbcdicFilePath = _fileDialog.OpenFileDialog("Select EBCDIC File"); });

            SelectCopybookFile =
                new DelegateCommand(() => { CopybookFilePath = _fileDialog.OpenFileDialog("Select Copybook XML File"); });

            EventAggregator.GetEvent<FinishReadEbcdicFileEvent>().Subscribe(FinishedReadingEbcdicFileHandler,
                ThreadOption.UIThread, false,
                e => e.EventType == ReadEbcdicFileEventType.ViewEbcdicFile);

            IsLoadingEbcdicFile = false;
        }

        public ICommand SelectEbcdicFile { get; private set; }
        public ICommand SelectCopybookFile { get; private set; }
        public DelegateCommand ReadEbcdicFile { get; private set; }

        public InteractionRequest<INotification> ErrorConfirmationRequest { get; private set; }

        #region "INotificationPropertyChanged"

        protected override void OnPropertyChanged(string propertyName = null)
        {
            if (propertyName == "EbcdicFilePath" || propertyName == "CopybookFilePath")
            {
                if (propertyName == "EbcdicFilePath")
                    UpdateCopybookFilePathIfMatchFound();

                ReadEbcdicFile.RaiseCanExecuteChanged();
            }

            base.OnPropertyChanged(propertyName);
        }

        #endregion

        #region "View Model Bindable Properties"

        public string EbcdicFilePath
        {
            get { return _ebcdicFilePath; }
            set
            {
                _ebcdicFilePath = value;
                OnPropertyChanged("EbcdicFilePath");
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

        public bool IsLoadingEbcdicFile
        {
            get { return _isLoadingEbcdicFile; }
            set
            {
                _isLoadingEbcdicFile = value;
                OnPropertyChanged("IsLoadingEbcdicFile");
            }
        }

        #endregion

        #region "Private Methods"

        private void ViewEbcdicFile()
        {
            IsLoadingEbcdicFile = true;

            EventAggregator.GetEvent<ViewEbcdicFileRequestEvent>().Publish(new ViewEbcdicFileRequest
            {
                EbcdicFilePath = EbcdicFilePath,
                CopybookFilePath = CopybookFilePath
            });
        }

        private bool CanViewEbcdicFile()
        {
            return !string.IsNullOrEmpty(EbcdicFilePath) && !string.IsNullOrEmpty(CopybookFilePath);
        }

        private void FinishedReadingEbcdicFileHandler(FinishReadEbcdicFile results)
        {
            IsLoadingEbcdicFile = false;

            if (!string.IsNullOrEmpty(results.ErrorMessage))
            {
                ErrorConfirmationRequest.Raise(
                    new Notification { Content = results.ErrorMessage, Title = "Oops! An error has occurred!" });
            }
        }

        private void UpdateCopybookFilePathIfMatchFound()
        {
            if (EbcdicFilePath == null)
                return;

            var ebcdicFileName = System.IO.Path.GetFileName(EbcdicFilePath);
            var copybookResult = _copybookManager.GetCopybookPathForEbcdicFile(ebcdicFileName);

            if (copybookResult.Result != null)
            {
                CopybookFilePath = copybookResult.Result;
            }
        }

        #endregion

        #region "BaseNavigationViewModel"

        protected override void OnResetView()
        {
            EbcdicFilePath = null;
            CopybookFilePath = null;
            IsLoadingEbcdicFile = false;

            EventAggregator.GetEvent<UpdateEbcdicFileGridEvent>().Publish(new UpdateEbcdicFileGridResult
            {
                Region = RegionNames.ViewEbcdicFileContentRegion,
                AllEbcdicFileRecordModels = new ObservableCollection<EbcdicFileRecordModel>(),
                VisibleEbcdicFileRecords = new ObservableCollection<EbcdicFileRecordModel>()
            });
        }

        #endregion
    }
}