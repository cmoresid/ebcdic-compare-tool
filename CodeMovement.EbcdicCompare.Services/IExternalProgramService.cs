namespace CodeMovement.EbcdicCompare.Services
{
    public interface IExternalProgramService
    {
        void RunProgram(string programName, params object[] args);
    }
}
