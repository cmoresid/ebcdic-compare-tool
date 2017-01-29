using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeMovement.EbcdicCompare.DataAccess
{
    public interface IFileOperation
    {
        string[] ReadAllLines(string filePath);
        byte[] ReadAllBytes(string filePath);
        void Copy(string fromPath, string toPath, bool overwrite);
        void Delete(string filePath);
        long GetFileSize(string filePath);
    }
}
