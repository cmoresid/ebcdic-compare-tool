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
        public OperationResult<bool> ExportRecordsToFile(FileExportRequest exportRequest)
        {
            var result = OperationResult<bool>.CreateResult(true);

            var csvRequest = exportRequest as CsvExportRequest;
            if (csvRequest == null)
                throw new ArgumentException("CsvFileExportService expects a CsvExportRequest object.");

            var groupedRecords = exportRequest.ExportedRecords
                .GroupBy(record => record.RecordTypeName,
                         (key, g) => new { RecordType = key, Records = g.AsEnumerable() })
                .ToList();

            try
            {
                foreach (var recordGroup in groupedRecords)
                    ExportRecordTypesToFile(exportRequest.OutputDirectory, recordGroup.RecordType, recordGroup.Records);
            }
            catch (Exception ex)
            {
                result.AddMessage("An error occurred while writing CSV file: " + ex.Message);
            }

            return result;
        }

        private void ExportRecordTypesToFile(
            string outputDirectory, 
            string recordType, 
            IEnumerable<EbcdicFileRecordModel> records)
        {
            var fileName = string.Format("{0}_{1}.csv", 
                recordType.Replace(' ', '_'), 
                DateTime.Now.ToString("yyyy_MM_dd"));

            var filePath = Path.Combine(outputDirectory, fileName);

            using (var textWriter = File.CreateText(filePath))
            {
                using (var writer = new CsvWriter(textWriter))
                {
                    var firstRecord = records.FirstOrDefault();
                    if (firstRecord == null)
                        return;

                    WriteHeader(writer, firstRecord);

                    foreach (var record in records)
                        WriteRecordByProperty(writer, record, (field) => field.Value);

                    textWriter.Flush();
                }
            }
        }

        private void WriteHeader(CsvWriter writer, EbcdicFileRecordModel firstRecord)
        {
            WriteRecordByProperty(writer, firstRecord, (field) => field.Format.Name);
        }

        private void WriteRecordByProperty(CsvWriter writer, EbcdicFileRecordModel record, Func<FieldValuePair, object> selector)
        {
            foreach (var field in record.RawFileRow.FieldValues)
                writer.WriteField(selector(field));

            writer.NextRecord();
        }
    }
}
