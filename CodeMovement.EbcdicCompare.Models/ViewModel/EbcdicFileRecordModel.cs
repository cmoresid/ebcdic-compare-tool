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
        public readonly EbcdicFileRow RawFileRow;
        private readonly IFieldFormat _fieldFormatter;

        public EbcdicFileRecordModel(IFieldFormat fieldFormatter, EbcdicFileRow fileRow)
        {
            if (fileRow == null)
                throw new ArgumentNullException("EbcdicFileRow cannot be null.");

            _fieldFormatter = fieldFormatter;

            RawFileRow = fileRow;
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
        public string Differences { get; set; }

        public bool ShowDifferences
        {
            get { return !string.IsNullOrWhiteSpace(Differences); }
        }

        public void PopulateColumnHeading()
        {
            var columnHeading = new StringBuilder();

            columnHeading.Append(FormatColumn(RawFileRow.FieldValues[0].Format.Name,
                _fieldFormatter.FormatField(RawFileRow.FieldValues[0])));

            foreach (var field in RawFileRow.FieldValues.Skip(1))
            {
                columnHeading.Append(" ");
                columnHeading.Append(FormatColumn(field.Format.Name,
                    _fieldFormatter.FormatField(field)));
            }

            ColumnHeading = columnHeading.ToString();
        }

        public static ObservableCollection<EbcdicFileRecordModel> Map(IFieldFormat formatter, List<EbcdicFileRow> ebcdicFileRows)
        {
            var ebcdicFileRowsViewModels = new ObservableCollection<EbcdicFileRecordModel>();

            if (ebcdicFileRows.Count == 0)
            {
                return ebcdicFileRowsViewModels;
            }

            var firstEbcdicFileRow = new EbcdicFileRecordModel(formatter, ebcdicFileRows[0]);
            var previousRecordTypeName = firstEbcdicFileRow.RecordTypeName;
            firstEbcdicFileRow.PopulateColumnHeading();

            ebcdicFileRowsViewModels.Add(firstEbcdicFileRow);

            var rowNumber = 2;
            foreach (var row in ebcdicFileRows.Skip(1))
            {
                var mapped = new EbcdicFileRecordModel(formatter, row);
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

            rowValue.Append(_fieldFormatter.FormatField(RawFileRow.FieldValues[0]));

            foreach (var field in RawFileRow.FieldValues.Skip(1))
            {
                rowValue.Append(" ");
                rowValue.Append(_fieldFormatter.FormatField(field));
            }

            return rowValue.ToString();
        }

        private string FormatColumn(string columnName, string fieldValue)
        {
            var formatString = "{0,-" + Math.Max(fieldValue.Length, columnName.Length) + "}";
            return string.Format(formatString, columnName);
        }

        #endregion
    }
}
