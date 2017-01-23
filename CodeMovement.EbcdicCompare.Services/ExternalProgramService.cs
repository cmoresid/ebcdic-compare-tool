using System.Diagnostics;
using System.Linq;

namespace CodeMovement.EbcdicCompare.Services
{
    public class ExternalProgramService : IExternalProgramService
    {
        public void RunProgram(string programName, params object[] args)
        {
            string arguments = args.Aggregate("", (argStr, arg) => argStr + " " + arg.ToString());

            var externalProcess = new Process
            {
                StartInfo =
                {
                    FileName = programName,
                    Arguments = arguments,
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Normal
                }
            };

            externalProcess.Start();
        }
    }
}
