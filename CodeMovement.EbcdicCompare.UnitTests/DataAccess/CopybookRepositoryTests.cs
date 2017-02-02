using CodeMovement.EbcdicCompare.DataAccess;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using System.IO;
using System.Linq;

namespace CodeMovement.EbcdicCompare.UnitTests.DataAccess
{
    /// <summary>
    /// Summary description for CopybookRepositoryTests
    /// </summary>
    [TestClass]
    public class CopybookRepositoryTests
    {
        #region "Test Context"

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #endregion

        private const string TestData = @".\TestData";

        protected string GetTestDbConnectionString(string dbName)
        {
            return string.Format("Data Source={0};Version=3", Path.Combine(TestData, "Dbs", dbName));
        }

        [TestMethod]
        public void CopybookRepository_Get_All_Copybooks()
        {
            var configuration = TestHelper.ConfigurationSettingsMock;
            configuration.Stub(m => m.CopybookDbConnectionString).Return(GetTestDbConnectionString("Copybook_Get.db"));

            var repository = new CopybookRepository(configuration);

            var results = repository.GetCopybooks();
            Assert.IsNotNull(results);

            var resultsAsList = results.ToList();
            Assert.AreEqual(1, resultsAsList.Count);

            var copybook = resultsAsList[0];
            Assert.AreEqual(@"C:\Copybook.fileformat", copybook.FilePath);
            Assert.AreEqual(2, copybook.AssociatedFiles.Count);
        }

        [TestMethod]
        public void CopybookRepository_Get_Copybook_For_Ebcdic_File()
        {
            var configuration = TestHelper.ConfigurationSettingsMock;
            configuration.Stub(m => m.CopybookDbConnectionString).Return(GetTestDbConnectionString("Copybook_Get.db"));

            var repository = new CopybookRepository(configuration);

            var results = repository.GetCopybookPathForEbcdicFile("EBCDICFILE2");
            Assert.IsNotNull(results);

            Assert.IsTrue(results.Successful);
            Assert.AreEqual(@"C:\Copybook.fileformat", results.Result);
        }

        [TestMethod]
        public void CopybookRepository_Add_Association()
        {
            var configuration = TestHelper.ConfigurationSettingsMock;
            configuration.Stub(m => m.CopybookDbConnectionString).Return(GetTestDbConnectionString("Copybook_Insert.db"));

            var repository = new CopybookRepository(configuration);

            var result = repository.AddCopybookEbcdicFileAssociation(@"C:\Copybook.fileformat", "EBCDICFILE1");

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Successful);

            var copybooks = repository.GetCopybooks();
            Assert.IsNotNull(result);
            Assert.AreEqual(1, copybooks.Count());
        }

        [TestMethod]
        public void CopybookRepository_Delete_Association()
        {
            var configuration = TestHelper.ConfigurationSettingsMock;
            configuration.Stub(m => m.CopybookDbConnectionString).Return(GetTestDbConnectionString("Copybook_Delete_Association.db"));

            var repository = new CopybookRepository(configuration);

            var result = repository.DeleteCopybookFileAssociation(@"C:\Copybook.fileformat", "EBCDICFILE1");
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Successful);

            var copybooks = repository.GetCopybooks();
            Assert.IsNotNull(copybooks);

            var copybooksAsList = copybooks.ToList();
            Assert.AreEqual(1, copybooksAsList.Count);
            Assert.AreEqual(@"C:\Copybook.fileformat", copybooksAsList[0].FilePath);
            Assert.AreEqual("EBCDICFILE2", copybooksAsList[0].AssociatedFiles[0]);
        }

        public void CopybookRepository_Delete_Copybook()
        {
            var configuration = TestHelper.ConfigurationSettingsMock;
            configuration.Stub(m => m.CopybookDbConnectionString).Return(GetTestDbConnectionString("Copybook_Delete_Association.db"));

            var repository = new CopybookRepository(configuration);

            var result = repository.DeleteCopybook(@"C:\Copybook2.fileformat");
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Successful);

            var copybooks = repository.GetCopybooks();
            Assert.IsNotNull(copybooks);

            var copybooksAsList = copybooks.ToList();
            Assert.AreEqual(1, copybooksAsList.Count);
            Assert.AreEqual(@"C:\Copybook1.fileformat", copybooksAsList[0].FilePath);
        }
    }
}
