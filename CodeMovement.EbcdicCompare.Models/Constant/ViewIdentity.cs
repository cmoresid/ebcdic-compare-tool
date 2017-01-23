using System;

namespace CodeMovement.EbcdicCompare.Models.Constant
{
    public static class ViewIdentity
    {
        public const string OpenEbcdicFileViewKey = "OpenEbcdicFileView";
        public const string CompareEbcdicFilesViewKey = "CompareEbcdicFilesView";
        public const string ManageCopybooksViewKey = "ManageCopybooksView";

        public static readonly Uri OpenEbcdicFileViewUrl = new Uri(OpenEbcdicFileViewKey, UriKind.Relative);
        public static readonly Uri CompareEbcdicFilesViewUrl = new Uri(CompareEbcdicFilesViewKey, UriKind.Relative);
        public static readonly Uri ManageCopybookXmlFilesViewUrl = new Uri(ManageCopybooksViewKey, UriKind.Relative);
    }
}
