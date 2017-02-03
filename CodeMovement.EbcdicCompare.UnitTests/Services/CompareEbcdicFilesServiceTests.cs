using Microsoft.VisualStudio.TestTools.UnitTesting;
using CodeMovement.EbcdicCompare.DataAccess;
using CodeMovement.EbcdicCompare.Services;
using Rhino.Mocks;
using System.IO;
using CodeMovement.EbcdicCompare.Models.Request;
using CodeMovement.EbcdicCompare.Models;

namespace CodeMovement.EbcdicCompare.Tests.Services
{
    [TestClass]
    public class CompareEbcdicFilesServiceTests
    {
        private const string TestData = @".\TestData";
        private const string Copybooks = TestData + @"\Copybooks";
        private const string EbcdicFiles = TestData + @"\EbcdicFiles";

        public ICompareEbcdicFilesService CompareEbcdicFilesService
        {
            get
            {
                return new CompareEbcdicFilesService(new EbcdicReaderService(), new FileOperation());
            }
        }

        [TestMethod]
        public void EbcdicCompareService_Compare_Bytes_Identical_Files()
        {
            var compareEbcdicFilesService = CompareEbcdicFilesService;
            var file1 = Path.Combine(EbcdicFiles, "Address.txt");
            var file2 = Path.Combine(EbcdicFiles, "Address.txt");

            var result = compareEbcdicFilesService.CompareEbcdicByteContents(file1, file2);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Result);
            Assert.IsNotNull(result.Successful);
            Assert.IsTrue(result.Result.AreIdentical);
            Assert.IsTrue(result.Result.FirstEbcdicFile.FileSize > 0);
            Assert.IsTrue(result.Result.SecondEbcdicFile.FileSize > 0);
            Assert.IsTrue(result.Result.FirstEbcdicFile.FileSize == result.Result.SecondEbcdicFile.FileSize);
        }

        [TestMethod]
        public void EbcdicCompareService_Compare_Bytes_Different_Files()
        {
            var compareEbcdicFilesService = CompareEbcdicFilesService;
            var file1 = Path.Combine(EbcdicFiles, "Address.txt");
            var file2 = Path.Combine(EbcdicFiles, "customer.txt");

            var result = compareEbcdicFilesService.CompareEbcdicByteContents(file1, file2);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Result);
            Assert.IsNotNull(result.Successful);
            Assert.IsFalse(result.Result.AreIdentical);
            Assert.IsTrue(result.Result.FirstEbcdicFile.FileSize > 0);
            Assert.IsTrue(result.Result.SecondEbcdicFile.FileSize > 0);
            Assert.IsFalse(result.Result.FirstEbcdicFile.FileSize == result.Result.SecondEbcdicFile.FileSize);
        }

        [TestMethod]
        public void EbcdicCompareService_Compare_Bytes_Exception()
        {
            var file1 = Path.Combine(EbcdicFiles, "Address.txt");
            var file2 = Path.Combine(EbcdicFiles, "customer.txt");

            var fileOperation = MockRepository.GenerateStub<IFileOperation>();
            fileOperation.Stub(m => m.GetFileSize(file1)).Throw(new FileNotFoundException());

            var compareEbcdicFilesService = new CompareEbcdicFilesService(new EbcdicReaderService(), fileOperation);

            var result = compareEbcdicFilesService.CompareEbcdicByteContents(file1, file2);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.Successful);
            Assert.AreEqual(1, result.Messages.Count);
        }

        [TestMethod]
        public void EbcdicCompareService_Compare_View_File_Simple()
        {
            var file1 = Path.Combine(EbcdicFiles, "simple.txt");
            var fileCopybook1 = Path.Combine(Copybooks, "simple.fileformat");

            var compareEbcdicFilesService = CompareEbcdicFilesService;

            var result = compareEbcdicFilesService.Compare(new CompareEbcdicFilesRequest()
            {
                FirstEbcdicFilePath = file1,
                CopybookFilePath = fileCopybook1
            });

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Successful);
            Assert.IsNotNull(result.Result);

            var compareResult = result.Result;

            Assert.IsNotNull(compareResult.FirstEbcdicFile);
            Assert.IsNotNull(compareResult.SecondEbcdicFile);

            var viewResult = compareResult.FirstEbcdicFile;

            Assert.IsNotNull(viewResult.EbcdicFileRecords);
            Assert.AreEqual(2, viewResult.EbcdicFileRecords.Count);

            Assert.AreEqual("LONG_VALUE FLOAT_VALUE DATE     BOOLEAN_VALUE", viewResult.EbcdicFileRecords[0].ColumnHeading);
            Assert.AreEqual("+001937    -0123.45    20141104 1            ", viewResult.EbcdicFileRecords[0].RowValue);
            Assert.AreEqual("SIMPLE", viewResult.EbcdicFileRecords[0].RecordTypeName);
            Assert.AreEqual(1, viewResult.EbcdicFileRecords[0].RowNumber);

            Assert.AreEqual(string.Empty, viewResult.EbcdicFileRecords[1].ColumnHeading);
            Assert.AreEqual("-000258    +0043.32    20140505 0            ", viewResult.EbcdicFileRecords[1].RowValue);
            Assert.AreEqual("SIMPLE", viewResult.EbcdicFileRecords[1].RecordTypeName);
            Assert.AreEqual(2, viewResult.EbcdicFileRecords[1].RowNumber);
        }

        [TestMethod]
        public void EbcdicCompareService_Compare_View_File_Multi_Record()
        {
            var file1 = Path.Combine(EbcdicFiles, "MultipleRecords.txt");
            var fileCopybook1 = Path.Combine(Copybooks, "MultipleRecords.fileformat");

            var compareEbcdicFilesService = CompareEbcdicFilesService;

            var result = compareEbcdicFilesService.Compare(new CompareEbcdicFilesRequest()
            {
                FirstEbcdicFilePath = file1,
                CopybookFilePath = fileCopybook1
            });

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Successful);
            Assert.IsNotNull(result.Result);

            var compareResult = result.Result;

            Assert.IsNotNull(compareResult.FirstEbcdicFile);
            Assert.IsNotNull(compareResult.SecondEbcdicFile);

            var viewResult = compareResult.FirstEbcdicFile;

            Assert.IsNotNull(viewResult.EbcdicFileRecords);
            Assert.AreEqual(4, viewResult.EbcdicFileRecords.Count);

            Assert.AreEqual("CODE   NAME                           DESCRIPTION                              TYPE DATE     CODE1  DESCRIPTION1                            ", viewResult.EbcdicFileRecords[0].ColumnHeading);
            Assert.AreEqual("+00020 Ebcdic Writer                   Ebcdic Writer                           100  20100101 +00020  Ebcdic Composite Writer                ", viewResult.EbcdicFileRecords[0].RowValue);
            Assert.AreEqual(1, viewResult.EbcdicFileRecords[0].RowNumber);

            Assert.AreEqual("CODE   NAME                           DESCRIPTION                              TYPE DATE     NAME1                          CODE1  DESCRIPTION1                            ", viewResult.EbcdicFileRecords[1].ColumnHeading);
            Assert.AreEqual("+00030 Ebcdic Writer1                  Ebcdic Writer1                          300  20100101 Writer1                        +00030  Ebcdic Composite Writer1               ", viewResult.EbcdicFileRecords[1].RowValue);
            Assert.AreEqual(2, viewResult.EbcdicFileRecords[1].RowNumber);

            Assert.AreEqual("CODE   NAME                           DESCRIPTION                              TYPE DATE     NAME1                          CODE1  DESCRIPTION1                            ", viewResult.EbcdicFileRecords[2].ColumnHeading);
            Assert.AreEqual("+00040 Ebcdic Writer2                  Ebcdic Writer2                          300  20100101 Writer2                        +00040  Ebcdic Composite Writer2               ", viewResult.EbcdicFileRecords[2].RowValue);
            Assert.AreEqual(3, viewResult.EbcdicFileRecords[2].RowNumber);

            Assert.AreEqual("CODE   NAME                           DESCRIPTION                              TYPE DATE     CODE1  DESCRIPTION1                            ", viewResult.EbcdicFileRecords[3].ColumnHeading);
            Assert.AreEqual("+00050 Ebcdic Writer3                  Ebcdic Writer3                          200  20100101 +00050  Ebcdic Composite Writer3               ", viewResult.EbcdicFileRecords[3].RowValue);
            Assert.AreEqual(4, viewResult.EbcdicFileRecords[3].RowNumber);
        }

        [TestMethod]
        public void EbcdicCompareService_Invalid_Copybook()
        {
            var file1 = Path.Combine(EbcdicFiles, "MultipleRecords.txt");
            var fileCopybook1 = Path.Combine(Copybooks, "Invalid.fileformat");

            var compareEbcdicFilesService = CompareEbcdicFilesService;

            var result = compareEbcdicFilesService.Compare(new CompareEbcdicFilesRequest()
            {
                FirstEbcdicFilePath = file1,
                CopybookFilePath = fileCopybook1
            });

            Assert.IsNotNull(result);
            Assert.IsFalse(result.Successful);
            Assert.AreEqual(1, result.Messages.Count);
            Assert.IsTrue(result.Messages[0].StartsWith("It looks like the selected copybook XML file you selected is not a valid XML file."));
        }

        [TestMethod]
        public void EbcdicCompareService_Compare_Same_Record_Length_Different_Values()
        {
            var file1 = Path.Combine(EbcdicFiles, "Person1_A.txt");
            var file2 = Path.Combine(EbcdicFiles, "Person1_B.txt");
            var copybook = Path.Combine(Copybooks, "Person.fileformat");

            var compareEbcdicFilesService = CompareEbcdicFilesService;

            var result = compareEbcdicFilesService.Compare(new CompareEbcdicFilesRequest
            {
                FirstEbcdicFilePath = file1,
                SecondEbcdicFilePath = file2,
                CopybookFilePath = copybook
            });

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Successful);
            Assert.IsNotNull(result.Result);

            var compareResult = result.Result;
            Assert.IsNotNull(compareResult.FirstEbcdicFile);
            Assert.IsNotNull(compareResult.SecondEbcdicFile);

            Assert.AreEqual(1, compareResult.FirstEbcdicFile.Differences);
            Assert.AreEqual(1, compareResult.FirstEbcdicFile.Matches);
            Assert.AreEqual(2, compareResult.FirstEbcdicFile.Total);

            Assert.AreEqual(RecordFlag.Identical, compareResult.FirstEbcdicFile.EbcdicFileRecords[0].Flag);
            Assert.AreEqual(RecordFlag.Different, compareResult.FirstEbcdicFile.EbcdicFileRecords[1].Flag);

            Assert.AreEqual(RecordFlag.Identical, compareResult.SecondEbcdicFile.EbcdicFileRecords[0].Flag);
            Assert.AreEqual(RecordFlag.Different, compareResult.SecondEbcdicFile.EbcdicFileRecords[1].Flag);
        }

        [TestMethod]
        public void EbcdicCompareService_Compare_Different_Record_Length_Same_Values()
        {
            var file1 = Path.Combine(EbcdicFiles, "Person1_A.txt");
            var file2 = Path.Combine(EbcdicFiles, "Person2.txt");
            var copybook = Path.Combine(Copybooks, "Person.fileformat");

            var compareEbcdicFilesService = CompareEbcdicFilesService;

            var result = compareEbcdicFilesService.Compare(new CompareEbcdicFilesRequest
            {
                FirstEbcdicFilePath = file1,
                SecondEbcdicFilePath = file2,
                CopybookFilePath = copybook
            });

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Successful);
            Assert.IsNotNull(result.Result);

            var compareResult = result.Result;
            Assert.IsNotNull(compareResult.FirstEbcdicFile);
            Assert.IsNotNull(compareResult.SecondEbcdicFile);

            Assert.AreEqual(RecordFlag.Identical, compareResult.FirstEbcdicFile.EbcdicFileRecords[0].Flag);
            Assert.AreEqual(RecordFlag.Identical, compareResult.FirstEbcdicFile.EbcdicFileRecords[1].Flag);

            Assert.AreEqual(RecordFlag.Identical, compareResult.SecondEbcdicFile.EbcdicFileRecords[0].Flag);
            Assert.AreEqual(RecordFlag.Identical, compareResult.SecondEbcdicFile.EbcdicFileRecords[1].Flag);

            Assert.AreEqual(RecordFlag.Extra, compareResult.SecondEbcdicFile.EbcdicFileRecords[2].Flag);
        }

        [TestMethod]
        public void EbcdicCompareService_Compare_And_Mark_Differences()
        {
            var file1 = Path.Combine(EbcdicFiles, "Person1_A.txt");
            var file2 = Path.Combine(EbcdicFiles, "Person1_B.txt");
            var copybook = Path.Combine(Copybooks, "Person.fileformat");

            var compareEbcdicFilesService = CompareEbcdicFilesService;

            var result = compareEbcdicFilesService.Compare(new CompareEbcdicFilesRequest
            {
                FirstEbcdicFilePath = file1,
                SecondEbcdicFilePath = file2,
                CopybookFilePath = copybook
            });

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Successful);
            Assert.IsNotNull(result.Result);

            var compareResults = result.Result;
            var file1Records = compareResults.FirstEbcdicFile.EbcdicFileRecords;
            var file2Records = compareResults.SecondEbcdicFile.EbcdicFileRecords;

            Assert.IsTrue(string.IsNullOrWhiteSpace(file1Records[0].Differences));
            Assert.IsTrue(string.IsNullOrWhiteSpace(file2Records[0].Differences));
            Assert.IsFalse(file1Records[0].ShowDifferences);
            Assert.IsFalse(file2Records[0].ShowDifferences);

            Assert.IsFalse(string.IsNullOrWhiteSpace(file1Records[1].Differences));
            Assert.IsFalse(string.IsNullOrWhiteSpace(file2Records[1].Differences));
            Assert.IsTrue(file1Records[1].ShowDifferences);
            Assert.IsTrue(file2Records[1].ShowDifferences);
            

            Assert.AreEqual("          ^^^^^^ ^          ^ ", file1Records[1].Differences);
            Assert.AreEqual("          ^^^^^^ ^          ^ ", file2Records[1].Differences);
        }
    }
}
