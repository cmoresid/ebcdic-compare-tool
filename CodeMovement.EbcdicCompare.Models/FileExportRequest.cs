using CodeMovement.EbcdicCompare.Models.ViewModel;
using System.Collections.Generic;

namespace CodeMovement.EbcdicCompare.Models
{
    /// <summary>
    /// The request object for the IFileExportService.
    /// </summary>
    public abstract class FileExportRequest
    {
        /// <summary>
        /// The records to export to a file.
        /// </summary>
        public IEnumerable<EbcdicFileRecordModel> ExportedRecords { get; set; }
        /// <summary>
        /// If set to true, then an EBCDIC that contains multiple record
        /// types, then create a separate file for each record type.
        /// </summary>
        public bool SplitMultiRecordFile { get; set; }

        /// <summary>
        /// The directory to export the file(s) to.
        /// </summary>
        public string OutputDirectory { get; set; }
    }
}
