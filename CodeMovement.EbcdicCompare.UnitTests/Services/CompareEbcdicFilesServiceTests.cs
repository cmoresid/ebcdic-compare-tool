using Microsoft.VisualStudio.TestTools.UnitTesting;
using CodeMovement.EbcdicCompare.DataAccess;
using CodeMovement.EbcdicCompare.Services;
using Rhino.Mocks;
using System.IO;

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
    }
}
