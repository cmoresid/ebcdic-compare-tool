using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CodeMovement.EbcdicCompare.Services;
using System.IO;
using CodeMovement.EbcdicCompare.Models.Request;
using CodeMovement.EbcdicCompare.Models.Result;
using CodeMovement.EbcdicCompare.Models.ViewModel;
using CodeMovement.EbcdicCompare.Models;

namespace CodeMovement.EbcdicCompare.UnitTests.Services
{
    [TestClass]
    public sealed class CsvFileExportServiceTests
    {
        private List<EbcdicFileRecordModel> ParseEbcdicFile(string fileName, string copybookName)
        {
            var file1 = Path.Combine(TestHelper.EbcdicFiles, fileName);
            var fileCopybook1 = Path.Combine(TestHelper.Copybooks, copybookName);

            var compareEbcdicFilesService = TestHelper.CompareEbcdicFilesService;

            var result = compareEbcdicFilesService.Compare(new CompareEbcdicFilesRequest()
            {
                FirstEbcdicFilePath = file1,
                CopybookFilePath = fileCopybook1
            });

            if (!result.Successful)
                throw new Exception("Unable to read EBCDIC file.");
            
            return result.Result.FirstEbcdicFile.EbcdicFileRecords.ToList();
        }


        [TestMethod]
        public void TestMethod1()
        {
            var fileName = "simple.txt";
            var copybookName = "simple.fileformat";

            var records = ParseEbcdicFile(fileName, copybookName);

            var csvWriter = new CsvFileExportService();
            var result = csvWriter.ExportRecordsToFile(new CsvExportRequest(records, @"C:\Users\cmoreside\Desktop"));
        }
    }
}
