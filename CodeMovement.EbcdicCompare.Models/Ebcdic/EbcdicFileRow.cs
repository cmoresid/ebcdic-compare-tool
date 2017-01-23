using System.Collections.Generic;

namespace CodeMovement.EbcdicCompare.Models.Ebcdic
{
    public class EbcdicFileRow
    {
        public string RecordTypeName { get; set; }
        public List<FieldValuePair> FieldValues { get; set; }
    }
}
