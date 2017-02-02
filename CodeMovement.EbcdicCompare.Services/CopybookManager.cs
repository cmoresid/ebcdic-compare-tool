using System.Collections.Generic;
using CodeMovement.EbcdicCompare.DataAccess;
using CodeMovement.EbcdicCompare.Models;
using CodeMovement.EbcdicCompare.Models.Result;

namespace CodeMovement.EbcdicCompare.Services
{
    public class CopybookManager : ICopybookManager
    {
        private readonly ICopybookRepository _copybookRepository;
        private readonly IFileOperationsManager _fileOperationsManager;
        private readonly IConfigurationSettings _configurationSettings;

        public CopybookManager(ICopybookRepository copybookRepository, 
            IConfigurationSettings configurationSettings,
            IFileOperationsManager fileOperationsManager)
        {
            _copybookRepository = copybookRepository;
            _fileOperationsManager = fileOperationsManager;
            _configurationSettings = configurationSettings;
        }

        public OperationResult<bool> AddCopybookEbcdicFileAssociation(string copybookName, string ebcdicFileName)
        {
            if (GetCopybookPathForEbcdicFile(ebcdicFileName).Result != null)
                return OperationResult<bool>.CreateResult(false, "EBCDIC file is already associated with a copybook.");

            var addCopybookFileResult = _fileOperationsManager.CopyFile(copybookName, _configurationSettings.CopybookFolderPath);

            return addCopybookFileResult.Result != null 
                ? _copybookRepository.AddCopybookEbcdicFileAssociation(addCopybookFileResult.Result, ebcdicFileName) 
                : OperationResult<bool>.CreateResult(false, addCopybookFileResult.Messages[0]);
        }

        public OperationResult<bool> DeleteCopybookFileAssociation(string copybookName, string ebcdicFileName)
        {
            return _copybookRepository.DeleteCopybookFileAssociation(copybookName, ebcdicFileName);
        }

        public OperationResult<bool> DeleteCopybook(string copybookName)
        {
            var deleteCopybookResult = _copybookRepository.DeleteCopybook(copybookName);

            if (deleteCopybookResult.Result)
                deleteCopybookResult = _fileOperationsManager.DeleteFile(copybookName);

            return deleteCopybookResult;
        }

        public OperationResult<string> GetCopybookPathForEbcdicFile(string ebcdicFileName)
        {
            var fileName = System.IO.Path.GetFileName(ebcdicFileName);
            return _copybookRepository.GetCopybookPathForEbcdicFile(fileName);
        }

        public IEnumerable<CopybookAssociation> GetCopybooks()
        {
            return _copybookRepository.GetCopybooks();
        }
    }
}
