using System.Collections.Generic;

namespace CodeMovement.EbcdicCompare.Models.Request
{
    public enum EbcdicRecordGrid
    {
        LegacyGrid,
        ModernizedGrid,
        Both
    }

    public class FilterEbcdicRecordsRequest
    {
        public EbcdicRecordGrid Target { get; set; }
        public List<RecordFlag> FilterBy { get; set; }
    }
}
