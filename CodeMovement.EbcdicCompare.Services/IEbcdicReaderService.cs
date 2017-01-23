using System.Collections.Generic;
using CodeMovement.EbcdicCompare.Models.Ebcdic;

namespace CodeMovement.EbcdicCompare.Services
{
    public interface IEbcdicReaderService
    {
        List<EbcdicFileRow> ReadEbcdicFile(string ebcdicFilePath, string copybookPath);
    }
}
