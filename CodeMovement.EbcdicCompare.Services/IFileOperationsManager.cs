using CodeMovement.EbcdicCompare.Models.Result;

namespace CodeMovement.EbcdicCompare.Services
{
    public interface IFileOperationsManager
    {
        OperationResult<string> ReadFileAsString(string filePath);
        OperationResult<byte[]> ReadFileAsBytes(string filePath);    
        OperationResult<string> CopyFile(string fromPath, string toPath);
        OperationResult<bool> DeleteFile(string filePath);
    }
}
