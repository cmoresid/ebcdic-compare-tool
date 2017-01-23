using System.Collections.Generic;

namespace CodeMovement.EbcdicCompare.Models.Constant
{
    public class Encoding
    {
        private Encoding(string value)
        {
            Value = value;
        }

        public string Value { get; set; }

        public static Encoding Ascii { get {  return new Encoding("ASCII"); } }
        public static Encoding Ebcdic { get { return new Encoding("EBCDIC"); } }

        public static List<string> AvailableEncodings 
        {
            get { return new List<string> { Ascii }; }
        }

        public override string ToString()
        {
            return Value;
        }

        public static implicit operator string(Encoding encoding)
        {
            return encoding.Value;
        }
    }
}
