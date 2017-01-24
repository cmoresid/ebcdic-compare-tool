using System.Collections.ObjectModel;
using Prism.Mvvm;
using CodeMovement.EbcdicCompare.Models.ViewModel;

namespace CodeMovement.EbcdicCompare.Presentation.ViewModel
{
    public class EbcdicFileGridViewModel : BindableBase
    {
        private ObservableCollection<EbcdicFileRecordModel> _visibleEbcdicFileRecords;

        public ObservableCollection<EbcdicFileRecordModel> AllEbcdicFileRecordModels { get; set; }

        public ObservableCollection<EbcdicFileRecordModel> VisibleEbcdicFileRecords
        {
            get { return _visibleEbcdicFileRecords; }
            set
            {
                _visibleEbcdicFileRecords = value;
                OnPropertyChanged();
            }
        }
    }
}
