namespace CodeMovement.EbcdicCompare.DataAccess
{
    public class FileOperation : IFileOperation
    {
        public void Copy(string fromPath, string toPath, bool overwrite)
        {
            System.IO.File.Copy(fromPath, toPath, overwrite);
        }

        public void Delete(string filePath)
        {
            System.IO.File.Delete(filePath);
        }

        public byte[] ReadAllBytes(string filePath)
        {
            return System.IO.File.ReadAllBytes(filePath);
        }

        public string[] ReadAllLines(string filePath)
        {
            return System.IO.File.ReadAllLines(filePath);
        }

        public long GetFileSize(string filePath)
        {
            return new System.IO.FileInfo(filePath).Length;
        }
    }
}
