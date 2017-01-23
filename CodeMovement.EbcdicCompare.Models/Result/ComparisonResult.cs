namespace CodeMovement.EbcdicCompare.Models.Result
{
    public class ComparisonResult
    {
        public ComparisonResult(bool areTheSame, long fileSize1, long fileSize2)
        {
            AreTheSame = areTheSame;
            FileSize1 = fileSize1;
            FileSize2 = fileSize2;
        }

        public bool AreTheSame { get; set; }
        public long FileSize1 { get; set; }
        public long FileSize2 { get; set; }
    }
}
