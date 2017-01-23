using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CodeMovement.EbcdicCompare.Models.Ebcdic;

namespace CodeMovement.EbcdicCompare.Models.ViewModel
{
    public class EbcdicFileRecordModel
    {
        private readonly EbcdicFileRow _ebcdicFileRow;

        public EbcdicFileRecordModel(EbcdicFileRow fileRow)
        {
            if (fileRow == null)
                throw new ArgumentNullException("EbcdicFileRow cannot be null.");

            _ebcdicFileRow = fileRow;

            ColumnHeading = string.Empty;
            RecordTypeName = fileRow.RecordTypeName;
            RowValue = Format();
            RowNumber = 1;
            Flag = RecordFlag.None;
        }

        public int RowNumber { get; set; }
        public RecordFlag Flag { get; set; }
        public string RecordTypeName { get; set; }
        public string ColumnHeading { get; set; }
        public string RowValue { get; set; }

        public void PopulateColumnHeading()
        {
            var columnHeading = new StringBuilder();

            columnHeading.Append(FormatColumn(_ebcdicFileRow.FieldValues[0].Format.Name, Format(_ebcdicFileRow.FieldValues[0])));

            for (int i = 1; i < _ebcdicFileRow.FieldValues.Count; i++)
            {
                columnHeading.Append(" ");
                columnHeading.Append(FormatColumn(_ebcdicFileRow.FieldValues[i].Format.Name, Format(_ebcdicFileRow.FieldValues[i])));
            }

            ColumnHeading = columnHeading.ToString();
        }

        public static ObservableCollection<EbcdicFileRecordModel> Map(List<EbcdicFileRow> ebcdicFileRows)
        {
            var ebcdicFileRowsViewModels = new ObservableCollection<EbcdicFileRecordModel>();

            if (ebcdicFileRows.Count == 0)
            {
                return ebcdicFileRowsViewModels;
            }

            var firstEbcdicFileRow = new EbcdicFileRecordModel(ebcdicFileRows[0]);
            var previousRecordTypeName = firstEbcdicFileRow.RecordTypeName;
            firstEbcdicFileRow.PopulateColumnHeading();

            ebcdicFileRowsViewModels.Add(firstEbcdicFileRow);

            var rowNumber = 2;
            foreach (var row in ebcdicFileRows.Skip(1))
            {
                var mapped = new EbcdicFileRecordModel(row);
                mapped.RowNumber = rowNumber++;

                if (previousRecordTypeName != mapped.RecordTypeName)
                {
                    mapped.PopulateColumnHeading();
                    previousRecordTypeName = mapped.RecordTypeName;
                }

                ebcdicFileRowsViewModels.Add(mapped);
            }

            return ebcdicFileRowsViewModels;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var record = obj as EbcdicFileRecordModel;
            return (record != null && RowValue == record.RowValue);
        }

        public override int GetHashCode()
        {
            return RowValue.GetHashCode();
        }

        #region "Private Methods"

        private string Format()
        {
            var rowValue = new StringBuilder();

            rowValue.Append(Format(_ebcdicFileRow.FieldValues[0]));

            foreach (var field in _ebcdicFileRow.FieldValues.Skip(1))
            {
                rowValue.Append(" ");
                rowValue.Append(Format(field));
            }

            return rowValue.ToString();
        }

        private string Format(FieldValuePair field)
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

        private string FormatColumn(string columnName, string fieldValue)
        {
            var formatString = "{0,-" + Math.Max(fieldValue.Length, columnName.Length) + "}";
            return string.Format(formatString, columnName);
        }

        #endregion
    }
}
