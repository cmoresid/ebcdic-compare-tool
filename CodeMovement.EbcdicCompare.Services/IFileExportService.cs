using CodeMovement.EbcdicCompare.Models;
using CodeMovement.EbcdicCompare.Models.Result;

namespace CodeMovement.EbcdicCompare.Services
{
    public interface IFileExportService
    {
        OperationResult<bool> ExportRecordsToFile(FileExportRequest exportRequest);
    }
}
