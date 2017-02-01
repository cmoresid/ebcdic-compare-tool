using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Regions;
using CodeMovement.EbcdicCompare.Models;
using CodeMovement.EbcdicCompare.Models.Result;
using CodeMovement.EbcdicCompare.Services;
using CodeMovement.EbcdicCompare.Presentation.Interaction;

namespace CodeMovement.EbcdicCompare.Presentation.ViewModel
{
    public class ManageCopybooksViewModel : BaseNavigationViewModel
    {
        #region "Attributes"

        private readonly ICopybookManager _copybookManager;
        private readonly IFileDialogInteraction _fileDialogInteraction;
        private readonly IFileOperationsManager _fileOperationsManager;

        private string _selectedNewCopybookFile;
        private string _selectedNewEbcdicFile;

        private CopybookAssociation _selectedCopybook;
        private string _selectedExistingEbcdicFile;
        private ObservableCollection<CopybookAssociation> _copybooks;
        private ObservableCollection<string> _associatedEbcdicFiles;
        private bool _associationSuccess;

        private static readonly WindowSize ManageCopybooksWindowSize = new WindowSize(800, 580);

        #endregion

        public ManageCopybooksViewModel(IEventAggregator eventAggregator,
            IRegionManager regionManager,
            IFileDialogInteraction fileDialogInteraction,
            IFileOperationsManager fileOperationsManager,
            ICopybookManager copybookManager)
            : base(regionManager, eventAggregator, ManageCopybooksWindowSize)
        {
            _copybookManager = copybookManager;
            _fileDialogInteraction = fileDialogInteraction;
            _fileOperationsManager = fileOperationsManager;

            ErrorConfirmationRequest = new InteractionRequest<INotification>();
            ShowCopybookRequest = new InteractionRequest<INotification>();

            ViewCopybookCommand = new DelegateCommand(OnViewCopybook, CanDeleteExistingCopybook);
            DeleteExistingCopybookCommand = new DelegateCommand(OnDeleteExistingCopybook, CanDeleteExistingCopybook);
            DeleteExistingEbcdicFileCommand = new DelegateCommand(OnDeleteExistingEbcdicFile, CanDeleteExistingEbcdicFile);
            SelectedExistingCopybookChangedCommand = new DelegateCommand<SelectionChangedEventArgs>(OnSelectedExistingCopybookChanged);
            SelectedExistingEbcicFileChangedCommand = new DelegateCommand<SelectionChangedEventArgs>(OnSelectedExistingEbcicFileChanged);

            AddAssociationCommand = new DelegateCommand(OnAddAssociation, CanAddAssociation);
            SelectNewCopybookCommand = new DelegateCommand(OnSelectNewCopybook);
            SelectNewEbcdicFileCommand = new DelegateCommand(OnSelectNewEbcdicFile);

            Copybooks = GetCopybooks();
            WasAssociationSuccessful = false;
        }

        public DelegateCommand DeleteExistingCopybookCommand { get; private set; }
        public DelegateCommand DeleteExistingEbcdicFileCommand { get; private set; }
        public DelegateCommand ViewCopybookCommand { get; private set; }
        public DelegateCommand<SelectionChangedEventArgs> SelectedExistingCopybookChangedCommand { get; private set; }
        public DelegateCommand<SelectionChangedEventArgs> SelectedExistingEbcicFileChangedCommand { get; private set; }

        public DelegateCommand SelectNewCopybookCommand { get; private set; }
        public DelegateCommand SelectNewEbcdicFileCommand { get; private set; }
        public DelegateCommand AddAssociationCommand { get; private set; }

        public InteractionRequest<INotification> ErrorConfirmationRequest { get; private set; }
        public InteractionRequest<INotification> ShowCopybookRequest { get; private set; }

        #region "View Model Bindable Properties"

        public string SelectedNewCopybookFile
        {
            get { return _selectedNewCopybookFile; }
            set
            {
                _selectedNewCopybookFile = value;
                OnPropertyChanged("SelectedNewCopybookFile");
            }
        }

        public string SelectedNewEbcdicFile
        {
            get { return _selectedNewEbcdicFile; }
            set
            {
                _selectedNewEbcdicFile = value;
                OnPropertyChanged("SelectedNewEbcdicFile");
            }
        }

        public CopybookAssociation SelectedCopybook
        {
            get { return _selectedCopybook; }
            set
            {
                _selectedCopybook = value;
                OnPropertyChanged("SelectedCopybook");

                AssociatedEbcdicFiles = (_selectedCopybook != null)
                    ? new ObservableCollection<string>(_selectedCopybook.AssociatedFiles)
                    : null;
            }
        }

        public string SelectedExistingEbcdicFile
        {
            get { return _selectedExistingEbcdicFile; }
            set
            {
                _selectedExistingEbcdicFile = value;
                OnPropertyChanged("SelectedExistingEbcdicFile");
            }
        }

        public ObservableCollection<string> AssociatedEbcdicFiles
        {
            get { return _associatedEbcdicFiles; }
            set
            {
                _associatedEbcdicFiles = value;
                OnPropertyChanged("AssociatedEbcdicFiles");
            }
        }

        public ObservableCollection<CopybookAssociation> Copybooks
        {
            get { return _copybooks; }
            set
            {
                _copybooks = value;
                OnPropertyChanged("Copybooks");
            }
        }

        public bool WasAssociationSuccessful
        {
            get { return _associationSuccess; }
            set
            {
                _associationSuccess = value;
                OnPropertyChanged("WasAssociationSuccessful");
            }
        }

        #endregion

        #region "INotificationPropertyChanged"

        protected override void OnPropertyChanged(string propertyName = null)
        {
            if (propertyName == "SelectedNewEbcdicFile" || propertyName == "SelectedNewCopybookFile")
            {
                WasAssociationSuccessful = false;
                AddAssociationCommand.RaiseCanExecuteChanged();
            }

            if (propertyName == "SelectedCopybook")
            {
                SelectedExistingEbcdicFile = null;
                DeleteExistingCopybookCommand.RaiseCanExecuteChanged();
                DeleteExistingEbcdicFileCommand.RaiseCanExecuteChanged();
                ViewCopybookCommand.RaiseCanExecuteChanged();
            }

            if (propertyName == "SelectedExistingEbcdicFile")
            {
                DeleteExistingEbcdicFileCommand.RaiseCanExecuteChanged();
            }

            base.OnPropertyChanged(propertyName);
        }

        #endregion

        #region "Event Handlers"

        private void OnAddAssociation()
        {
            var associationResult =
                _copybookManager.AddCopybookEbcdicFileAssociation(SelectedNewCopybookFile, SelectedNewEbcdicFile);

            HandleCopybookOperationResult(associationResult);

            WasAssociationSuccessful = associationResult.Result;
        }

        private bool CanAddAssociation()
        {
            return !(string.IsNullOrEmpty(SelectedNewCopybookFile) || string.IsNullOrEmpty(SelectedNewEbcdicFile));
        }

        private void OnDeleteExistingCopybook()
        {
            var deleteCopybookResult = _copybookManager.DeleteCopybook(SelectedCopybook.FilePath);

            HandleCopybookOperationResult(deleteCopybookResult);
        }

        private bool CanDeleteExistingCopybook()
        {
            return SelectedCopybook != null;
        }

        private void OnDeleteExistingEbcdicFile()
        {
            var deleteEbcdicFileResult = _copybookManager.DeleteCopybookFileAssociation(SelectedCopybook.FilePath,
                SelectedExistingEbcdicFile);

            HandleCopybookOperationResult(deleteEbcdicFileResult);
        }

        private bool CanDeleteExistingEbcdicFile()
        {
            return SelectedExistingEbcdicFile != null;
        }

        private void OnSelectedExistingCopybookChanged(SelectionChangedEventArgs e)
        {
            SelectedCopybook = (e.AddedItems.Count > 0)
                ? e.AddedItems[0] as CopybookAssociation
                : null;
        }

        private void OnSelectedExistingEbcicFileChanged(SelectionChangedEventArgs e)
        {
            SelectedExistingEbcdicFile = (e.AddedItems.Count > 0)
                ? e.AddedItems[0] as string
                : null;
        }

        private void OnViewCopybook()
        {
            var copybookContentsResults = _fileOperationsManager.ReadFileAsString(SelectedCopybook.FilePath);

            if (copybookContentsResults.Result != null)
            {
                ShowCopybookRequest.Raise(new Notification
                {
                    Content = copybookContentsResults.Result,
                    Title = "Copybook Contents"
                });
            }
            else
            {
                ErrorConfirmationRequest.Raise(new Notification
                {
                    Title = "Oops! An error has occurred",
                    Content = copybookContentsResults.Messages[0]
                });
            }
        }

        private void OnSelectNewCopybook()
        {
            SelectedNewCopybookFile = _fileDialogInteraction.OpenFileDialog("Select New Copybook XML file");
        }

        private void OnSelectNewEbcdicFile()
        {
            var ebcdicFilePath = _fileDialogInteraction.OpenFileDialog("Select New EBCDIC file");

            SelectedNewEbcdicFile = ebcdicFilePath != null
                ? System.IO.Path.GetFileName(ebcdicFilePath)
                : null;
        }

        #endregion

        #region "Helper Methods"

        private void HandleCopybookOperationResult(OperationResult<bool> result)
        {
            if (!result.Result)
            {
                ErrorConfirmationRequest.Raise(
                    new Notification { Content = string.Join("\\n", result.Messages), Title = "Oops! Something went wrong!" });
            }
            else
            {
                Copybooks = new ObservableCollection<CopybookAssociation>(_copybookManager.GetCopybooks());
            }
        }

        private ObservableCollection<CopybookAssociation> GetCopybooks()
        {
            var copybooks = new ObservableCollection<CopybookAssociation>();

            try
            {
                var availableCopybooks = _copybookManager.GetCopybooks();
                copybooks.AddRange(availableCopybooks);
            }
            catch (Exception ex)
            {
                ErrorConfirmationRequest.Raise(
                    new Notification { Content = ex.Message, Title = "Oops! Something went wrong!" });
            }

            return copybooks;
        }

        #endregion

        #region "BaseNavigationViewModel"

        protected override void OnResetView()
        {
            SelectedCopybook = null;
            SelectedExistingEbcdicFile = null;
            AssociatedEbcdicFiles = null;
            SelectedNewEbcdicFile = null;
            SelectedNewCopybookFile = null;
            WasAssociationSuccessful = false;

            Copybooks = GetCopybooks();
        }

        #endregion
    }
}
