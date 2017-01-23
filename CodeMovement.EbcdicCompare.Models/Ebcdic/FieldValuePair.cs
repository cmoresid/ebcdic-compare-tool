using CodeMovement.EbcdicCompare.Models.Copybook;

namespace CodeMovement.EbcdicCompare.Models.Ebcdic
{
    public class FieldValuePair
    {
        public FieldValuePair(FieldFormat format, object value)
        {
            Format = format;
            Value = value;
        }

        public FieldFormat Format { get; set; }
        public object Value { get; set; }
    }
}
