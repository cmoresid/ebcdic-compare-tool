using System.Collections.Generic;
using CodeMovement.EbcdicCompare.Models;
using CodeMovement.EbcdicCompare.Models.Result;

namespace CodeMovement.EbcdicCompare.DataAccess
{
    public interface ICopybookRepository
    {
        OperationResult<bool> AddCopybookEbcdicFileAssociation(string copybookName, string ebcdicFileName);
        OperationResult<bool> DeleteCopybookFileAssociation(string copybookName, string ebcdicFileName);
        OperationResult<bool> DeleteCopybook(string copybookName);
        OperationResult<string> GetCopybookPathForEbcdicFile(string ebcdicFileName);

        IEnumerable<CopybookAssociation> GetCopybooks();
    }
}
