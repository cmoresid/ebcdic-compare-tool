using System.Collections.Generic;

namespace CodeMovement.EbcdicCompare.Models
{
    public class CopybookAssociation
    {
        public string FilePath { get; set; }
        public List<string> AssociatedFiles { get; set; }
    }
}
