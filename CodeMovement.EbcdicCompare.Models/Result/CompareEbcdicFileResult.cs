namespace CodeMovement.EbcdicCompare.Models.Result
{
    public class CompareEbcdicFileResult
    {
        public EbcdicFileAnalysis FirstEbcdicFile { get; set; }
        public EbcdicFileAnalysis SecondEbcdicFile { get; set; }
        public bool AreIdentical { get; set; }
    }
}
