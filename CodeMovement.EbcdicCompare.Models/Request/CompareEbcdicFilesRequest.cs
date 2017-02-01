using CodeMovement.EbcdicCompare.Models.Constant;

namespace CodeMovement.EbcdicCompare.Models.Request
{
    public class CompareEbcdicFilesRequest
    {
        public string FirstEbcdicFilePath { get; set; }
        public string SecondEbcdicFilePath { get; set; }
        public string CopybookFilePath { get; set; }
    }
}
