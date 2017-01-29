using System;
using System.IO;
using CodeMovement.EbcdicCompare.Models.Result;
using CodeMovement.EbcdicCompare.DataAccess;

namespace CodeMovement.EbcdicCompare.Services
{
    public class FileOperationsManager : IFileOperationsManager
    {
        private readonly IFileOperation _fileOperation;

        public FileOperationsManager(IFileOperation fileOperation)
        {
            _fileOperation = fileOperation;
        }

        public OperationResult<string> ReadFileAsString(string filePath)
        {
            var result = new OperationResult<string>();

            if (string.IsNullOrWhiteSpace(filePath))
                return OperationResult<string>.CreateResult(null, "Must specify file path.");

            try
            {
                result.Result = string.Join("\n", _fileOperation.ReadAllLines(filePath));
            }
            catch (Exception ex)
            {
                result.AddMessage(ex.Message);
            }

            return result;
        }

        public OperationResult<byte[]> ReadFileAsBytes(string filePath)
        {
            var result = new OperationResult<byte[]>();

            if (string.IsNullOrWhiteSpace(filePath))
                return OperationResult<byte[]>.CreateResult(null, "Must specify file path.");

            try
            {
                result.Result = _fileOperation.ReadAllBytes(filePath);
            }
            catch (Exception ex)
            {
                result.AddMessage(ex.Message);
            }

            return result;
        }

        public OperationResult<string> CopyFile(string fromPath, string toPath)
        {
            var result = new OperationResult<string>();

            if (string.IsNullOrWhiteSpace(fromPath))
                return OperationResult<string>.CreateResult(null, "fromPath must be specified.");

            if (string.IsNullOrWhiteSpace(toPath))
                return OperationResult<string>.CreateResult(null, "toPath must be specified.");

            try
            {
                var fileName = Path.GetFileName(fromPath);
                var newToPath = Path.Combine(toPath, fileName);

                _fileOperation.Copy(fromPath, newToPath, true);

                result.Result = newToPath;
            }
            catch (Exception ex)
            {
                result.Result = null;
                result.AddMessage(ex.Message);
            }

            return result;
        }

        public OperationResult<bool> DeleteFile(string filePath)
        {
            var result = OperationResult<bool>.CreateResult(true);

            if (string.IsNullOrWhiteSpace(filePath))
                return OperationResult<bool>.CreateResult(false, "filePath must be specified.");

            try
            {
                _fileOperation.Delete(filePath);
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.AddMessage(ex.Message);
            }

            return result;
        }

        public OperationResult<long> GetFileSize(string filePath)
        {
            var result = new OperationResult<long>();

            try
            {
                var fileSize = _fileOperation.GetFileSize(filePath);
                result.Result = fileSize;
            }
            catch (Exception ex)
            {
                result.Result = -1;
                result.AddMessage(ex.Message);
            }

            return result;
        }
    }
}
