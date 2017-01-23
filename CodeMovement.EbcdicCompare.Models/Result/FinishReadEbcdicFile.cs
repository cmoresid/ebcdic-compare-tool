using CodeMovement.EbcdicCompare.Models.Constant;

namespace CodeMovement.EbcdicCompare.Models.Result
{
    public class FinishReadEbcdicFile
    {
        public ReadEbcdicFileEventType EventType { get; set; }
        public string ErrorMessage { get; set; }
        public CompareEbcdicFileResult CompareEbcdicFileResult { get; set; }
    }
}
