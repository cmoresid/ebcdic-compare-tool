using System;
using System.IO;
using CodeMovement.EbcdicCompare.Models.Result;

namespace CodeMovement.EbcdicCompare.Services
{
    public class FileOperationsManager : IFileOperationsManager
    {
        public OperationResult<string> ReadFileAsString(string filePath)
        {
            var result = new OperationResult<string>();

            try
            {
                result.Result = string.Join("\n", File.ReadAllLines(filePath));
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
                result.Result = File.ReadAllBytes(filePath);
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

            try
            {
                var fileName = Path.GetFileName(fromPath);
                var newToPath = Path.Combine(toPath, fileName);

                // Do not attempt to overwrite the file with
                // itself.
                if (fromPath != newToPath)
                    File.Copy(fromPath, newToPath, true);

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

            try
            {
                File.Delete(filePath);
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.AddMessage(ex.Message);
            }

            return result;
        }
    }
}
