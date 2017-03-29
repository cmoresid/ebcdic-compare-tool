using CodeMovement.EbcdicCompare.Models.ViewModel;
using System.Collections.Generic;

namespace CodeMovement.EbcdicCompare.Models
{
    public class CsvExportRequest : FileExportRequest
    {
        public CsvExportRequest(IEnumerable<EbcdicFileRecordModel> exportedRecords, string outputDirectory)
        {
            Delimiter = ',';
            ExportedRecords = exportedRecords;
            OutputDirectory = outputDirectory;
        }

        public char Delimiter { get; set; }
    }
}
