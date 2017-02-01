using System.Collections.Generic;

namespace CodeMovement.EbcdicCompare.Models.Request
{
    public class FilterEbcdicRecordsRequest
    {
        public string RegionName { get; set; }
        public List<RecordFlag> FilterBy { get; set; }
    }
}
