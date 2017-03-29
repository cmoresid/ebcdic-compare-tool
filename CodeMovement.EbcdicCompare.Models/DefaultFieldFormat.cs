using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeMovement.EbcdicCompare.Models.Ebcdic;
using System.Text.RegularExpressions;

namespace CodeMovement.EbcdicCompare.Models
{
    public class DefaultFieldFormat : IFieldFormat
    {
        public string FormatField(FieldValuePair field)
        {
            var formatString = (field.Format.Type == "3" || field.Format.Type == "9") ?
                FormatDecimal(field) : FormatString(field);

            var formattedValue = string.Format(formatString, field.Value);
            var maxLength = Math.Max(formattedValue.Length, field.Format.Name.Length);

            return Regex.Escape(formattedValue.PadRight(maxLength))
                .Replace(@"\ ", " ")
                .Replace(@"\.", ".")
                .Replace(@"\+", "+")
                .Replace(@"\-", "-");
        }

        private static string FormatDecimal(FieldValuePair field)
        {
            const string formatString = "{0:pF;nF}";
            var decimalFormat = new StringBuilder();

            var precision = field.Format.Decimal;
            var scale = int.Parse(field.Format.Size) - precision;

            Enumerable.Repeat(0, scale).ToList().ForEach(x => decimalFormat.Append("0"));

            if (precision > 0)
            {
                decimalFormat.Append(".");
                Enumerable.Repeat(0, precision).ToList().ForEach(x => decimalFormat.Append("0"));
            }

            return formatString
                     .Replace("p", field.Format.Signed ? "+" : "")
                     .Replace("n", field.Format.Signed ? "-" : "")
                     .Replace("F", decimalFormat.ToString());
        }

        private string FormatString(FieldValuePair field)
        {
            return "{0,-" + field.Format.Size + "}";
        }
    }
}
