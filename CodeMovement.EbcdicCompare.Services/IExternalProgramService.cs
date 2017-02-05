using CodeMovement.EbcdicCompare.Models.Result;

namespace CodeMovement.EbcdicCompare.Services
{
    public interface IExternalProgramService
    {
        OperationResult<bool> RunProgram(string programName, params object[] args);
    }
}
