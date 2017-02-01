using CodeMovement.EbcdicCompare.Models.ViewModel;
using System.Collections.ObjectModel;

namespace CodeMovement.EbcdicCompare.Models.Result
{
    public class UpdateEbcdicFileGridResult
    {
        public string Region { get; set; }
        public ObservableCollection<EbcdicFileRecordModel> AllEbcdicFileRecordModels { get; set; }
        public ObservableCollection<EbcdicFileRecordModel> VisibleEbcdicFileRecords { get; set; }
    }
}
