namespace CodeMovement.EbcdicCompare.Presentation.ViewModel
{
    public partial class CompareEbcdicFilesViewModel
    {
        public enum States
        {
            Initial,
            ReadyToPerformInitialCompare,
            PerformingInitialCompare,
            FinishedInitialCompare,
            FilesMatch,
            FilesDoNotMatch,
            ShouldUseCopybook,
            UseCopybook,
            DoNotUseCopybook,
            ReadyToPerformCopybookCompare,
            PerformingCopybookCompare,
            FinishedCopybookCompare
        }

        public States CurrentState
        {
            get { return _currentState; }
            set
            {
                _currentState = value;
                OnSetState(_currentState);
            }
        }

        #region "Attributes"

        private bool _showFilesMatchLabel;
        private bool _showFilesDoNotMatchLabel;
        private bool _showInitialCompareIndicator;
        private bool _showCompareUsingCopybookRow;
        private bool _showOpenFilesInExternalEditorRow;
        private bool _showSelectCopybookRow;
        private bool _showCopybookCompareButtonRow;
        private bool _showCopybookCompareIndicator;
        private bool _finishedCompare;
        private bool _filterByRecordDifferences;

        private States _currentState;

        #endregion

        #region "State Properties"

        public bool ShowFilesMatchLabel
        {
            get { return _showFilesMatchLabel; }
            set { SetProperty(ref _showFilesMatchLabel, value); }
        }

        public bool ShowFilesDoNotMatchLabel
        {
            get { return _showFilesDoNotMatchLabel; }
            set { SetProperty(ref _showFilesDoNotMatchLabel, value); }
        }

        public bool ShowInitialCompareIndicator
        {
            get { return _showInitialCompareIndicator; }
            set { SetProperty(ref _showInitialCompareIndicator, value);  }
        }

        public bool ShowCompareUsingCopybookRow
        {
            get { return _showCompareUsingCopybookRow; }
            set { SetProperty(ref _showCompareUsingCopybookRow, value); }
        }

        public bool ShowOpenFilesInExternalEditorRow
        {
            get { return _showOpenFilesInExternalEditorRow; }
            set { SetProperty(ref _showOpenFilesInExternalEditorRow, value); }
        }

        public bool ShowSelectCopybookRow
        {
            get { return _showSelectCopybookRow; }
            set { SetProperty(ref _showSelectCopybookRow, value); }
        }

        public bool ShowCopybookCompareButtonRow
        {
            get { return _showCopybookCompareButtonRow; }
            set { SetProperty(ref _showCopybookCompareButtonRow, value); }
        }

        public bool ShowCopybookCompareIndicator
        {
            get { return _showCopybookCompareIndicator; }
            set { SetProperty(ref _showCopybookCompareIndicator, value); }
        }

        public bool FinishedCompare
        {
            get { return _finishedCompare; }
            set { SetProperty(ref _finishedCompare, value); }
        }

        #endregion

        #region "State Management"

        protected void OnSetState(States state)
        {
            switch (state)
            {
                case States.Initial:
                    LegacyEbcdicFilePath = null;
                    ModernizedEbcdicFilePath = null;
                    CopybookFilePath = null;
                    CompareEbcdicFileResult = null;
                    UseCopybook = null;
                    ShowSelectCopybookRow = false;
                    ShowCopybookCompareButtonRow = false;
                    ShowCompareUsingCopybookRow = false;
                    ShowCopybookCompareIndicator = false;
                    ShowFilesDoNotMatchLabel = false;
                    ShowFilesMatchLabel = false;
                    ShowInitialCompareIndicator = false;
                    ShowOpenFilesInExternalEditorRow = false;
                    FinishedCompare = false;
                    FilterByRecordDifferences = false;
                    break;
                case States.PerformingInitialCompare:
                    ShowInitialCompareIndicator = true;
                    ShowCompareUsingCopybookRow = false;
                    ShowOpenFilesInExternalEditorRow = false;
                    ShowSelectCopybookRow = false;
                    ShowCopybookCompareButtonRow = false;
                    ShowCopybookCompareIndicator = false;
                    break;
                case States.FinishedInitialCompare:
                    ShowInitialCompareIndicator = false;
                    break;
                case States.FilesDoNotMatch:
                    ShowFilesDoNotMatchLabel = true;
                    ShowFilesMatchLabel = false;
                    ShowCompareUsingCopybookRow = true;
                    break;
                case States.FilesMatch:
                    ShowFilesMatchLabel = true;
                    ShowFilesDoNotMatchLabel = false;
                    break;
                case States.ShouldUseCopybook:
                    CopybookFilePath = null;
                    ShowOpenFilesInExternalEditorRow = false;
                    ShowSelectCopybookRow = false;
                    ShowCopybookCompareButtonRow = false;
                    ShowCopybookCompareIndicator = false;
                    break;
                case States.UseCopybook:
                    ShowSelectCopybookRow = true;
                    break;
                case States.DoNotUseCopybook:
                    ShowOpenFilesInExternalEditorRow = true;
                    break;
                case States.ReadyToPerformCopybookCompare:
                    ShowCopybookCompareButtonRow = true;
                    break;
                case States.PerformingCopybookCompare:
                    ShowCopybookCompareIndicator = true;
                    break;
                case States.FinishedCopybookCompare:
                    ShowCopybookCompareIndicator = false;
                    FinishedCompare = true;
                    break;
            }
        }

        #endregion
    }
}
