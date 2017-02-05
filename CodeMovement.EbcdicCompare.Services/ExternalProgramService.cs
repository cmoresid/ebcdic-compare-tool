using CodeMovement.EbcdicCompare.Models.Result;
using System;
using System.Diagnostics;
using System.Linq;

namespace CodeMovement.EbcdicCompare.Services
{
    public class ExternalProgramService : IExternalProgramService
    {
        public OperationResult<bool> RunProgram(string programName, params object[] args)
        {
            OperationResult<bool> result = OperationResult<bool>.CreateResult(true);

            try
            {
                var arguments = args.Aggregate("", (argStr, arg) => argStr + " " + arg.ToString());

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
            catch (Exception ex)
            {
                result.AddMessage(ex.Message);
            }

            return result;
        }
    }
}
