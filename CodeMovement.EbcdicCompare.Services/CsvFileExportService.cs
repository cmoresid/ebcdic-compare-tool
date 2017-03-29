using CodeMovement.EbcdicCompare.Models;
using CodeMovement.EbcdicCompare.Models.Ebcdic;
using CodeMovement.EbcdicCompare.Models.Result;
using CodeMovement.EbcdicCompare.Models.ViewModel;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CodeMovement.EbcdicCompare.Services
{
    public class CsvFileExportService : IFileExportService
    {
        private readonly IFieldFormat _fieldFormatter;

        public CsvFileExportService(IFieldFormat fieldFormatter)
        {
            _fieldFormatter = fieldFormatter;
        }

        public OperationResult<bool> ExportRecordsToFile(FileExportRequest exportRequest)
        {
            var result = OperationResult<bool>.CreateResult(true);

            var csvRequest = exportRequest as CsvExportRequest;
            if (csvRequest == null)
                throw new ArgumentException("CsvFileExportService expects a CsvExportRequest object.");

            var groupedRecords = exportRequest.ExportedRecords
                .GroupBy(record => record.RawFileRow.RecordTypeName,
                         (key, g) => new { RecordType = key, Records = g.AsEnumerable() })
                .ToList();

            try
            {
                foreach (var recordGroup in groupedRecords)
                    ExportRecordTypesToFile(exportRequest.TrimTrailingWhiteSpace, 
                        exportRequest.OutputDirectory, recordGroup.RecordType, recordGroup.Records);
            }
            catch (Exception ex)
            {
                result.AddMessage("An error occurred while writing CSV file: " + ex.Message);
            }

            return result;
        }

        private void ExportRecordTypesToFile(
            bool trimWhiteSpace,
            string outputDirectory, 
            string recordType, 
            IEnumerable<EbcdicFileRecordModel> records)
        {
            var fileName = MakeValidFileName(string.Format("{0}_{1}.csv", 
                recordType.Replace(' ', '_'), 
                DateTime.Now.ToString("yyyy_MM_dd")));

            var filePath = Path.Combine(outputDirectory, fileName);

            using (var textWriter = File.CreateText(filePath))
            {
                using (var writer = new CsvWriter(textWriter))
                {
                    writer.Configuration.TrimFields = trimWhiteSpace;

                    var firstRecord = records.FirstOrDefault();
                    if (firstRecord == null)
                        return;

                    WriteHeader(writer, firstRecord);

                    foreach (var record in records)
                        WriteRecordByProperty(writer, record, (field) => _fieldFormatter.FormatField(field));

                    textWriter.Flush();
                }
            }
        }

        private void WriteHeader(CsvWriter writer, EbcdicFileRecordModel firstRecord)
        {
            WriteRecordByProperty(writer, firstRecord, (field) => field.Format.Name);
        }

        private void WriteRecordByProperty(CsvWriter writer, EbcdicFileRecordModel record, Func<FieldValuePair, string> selector)
        {
            foreach (var field in record.RawFileRow.FieldValues)
                writer.WriteField(selector(field));

            writer.NextRecord();
        }

        private static string MakeValidFileName(string name)
        {
            string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return System.Text.RegularExpressions.Regex.Replace(name, invalidRegStr, "_");
        }
    }
}
