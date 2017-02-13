using CodeMovement.EbcdicCompare.Models.Request;
using CodeMovement.EbcdicCompare.Models.Result;

namespace CodeMovement.EbcdicCompare.Services
{
    public interface ICompareEbcdicFilesService
    {
        OperationResult<CompareEbcdicFileResult> Compare(CompareEbcdicFilesRequest request);
        OperationResult<CompareEbcdicFileResult> CompareEbcdicByteContents(string ebcdicFilePath1, string ebcdicFilePath2);
        OperationResult<CompareEbcdicFileResult> SortCompareEbcdicResults(CompareEbcdicFileResult compareResults, bool sort = true);
    }
}
