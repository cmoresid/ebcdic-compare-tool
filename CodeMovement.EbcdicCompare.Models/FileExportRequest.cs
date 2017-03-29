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
        /// If set to true, then the trailing whitespace on each field
        /// value will be trimmed.
        /// </summary>
        public bool TrimTrailingWhiteSpace { get; set; }

        /// <summary>
        /// The directory to export the file(s) to.
        /// </summary>
        public string OutputDirectory { get; set; }
    }
}
