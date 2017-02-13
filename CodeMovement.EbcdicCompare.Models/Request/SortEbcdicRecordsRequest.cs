using CodeMovement.EbcdicCompare.Models.Result;

namespace CodeMovement.EbcdicCompare.Models.Request
{
    public class SortEbcdicRecordsRequest
    {
        public bool SortEbcdicFileRecords { get; set; }
        public CompareEbcdicFileResult CompareResult { get; set; }
    }
}
