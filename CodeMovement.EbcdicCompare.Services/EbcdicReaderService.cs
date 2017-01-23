using System.Collections.Generic;
using System.IO;
using System.Linq;
using CodeMovement.EbcdicCompare.Models.Ebcdic;
using CodeMovement.EbcdicCompare.Models.Result;

namespace CodeMovement.EbcdicCompare.Services
{
    public class EbcdicReaderService : IEbcdicReaderService
    {
        public List<EbcdicFileRow> ReadEbcdicFile(string ebcdicFilePath, string copybookPath)
        {
            var results = new List<EbcdicFileRow>();
            var ebcdicReader = new EbcdicFileReader(ebcdicFilePath, copybookPath);

            ebcdicReader.Open();

            var row = ebcdicReader.Read();
            while (row != null)
            {
                results.Add(row);
                row = ebcdicReader.Read();
            }

            return results;
        }
    }
}
