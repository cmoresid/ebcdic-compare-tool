using System.Collections.ObjectModel;
using CodeMovement.EbcdicCompare.Models.ViewModel;

namespace CodeMovement.EbcdicCompare.Models.Result
{
    public class EbcdicFileAnalysis
    {
        public ObservableCollection<EbcdicFileRecordModel> EbcdicFileRecords { get; set; }

        public int Total { get; set; }
        public int Matches { get; set; }
        public int Differences { get; set; }
        public int Extras { get; set; }
        public long FileSize { get; set; }
    }
}
