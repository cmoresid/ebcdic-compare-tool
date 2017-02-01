using CodeMovement.EbcdicCompare.Models.Constant;

namespace CodeMovement.EbcdicCompare.Models.Request
{
    public class ViewEbcdicFileRequest
    {
        public string EbcdicFilePath { get; set; }
        public string CopybookFilePath { get; set; }
    }
}
