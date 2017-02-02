using System.Collections.Generic;
using System.IO;
using Rhino.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CodeMovement.EbcdicCompare.DataAccess;
using CodeMovement.EbcdicCompare.Models.Result;
using CodeMovement.EbcdicCompare.Services;
using CodeMovement.EbcdicCompare.UnitTests;

namespace CodeMovement.EbcdicCompare.Tests.Services
{
    [TestClass]
    public class CopybookManagerTests
    {
        #region "Helper Methods"

        protected string CopybookTestDataPath
        {
            get { return Path.Combine("TestData", "Copybooks"); }
        }

        #endregion

        [TestMethod]
        public void Does_CopybookManager_Delete_Association()
        {
            var copybookRepositoryMock = TestHelper.CopybookRepositoryMock;
            copybookRepositoryMock.Expect(m => m.DeleteCopybookFileAssociation("COPYBOOK1", "EBCDICFILE1"));

            ICopybookManager copybookManager = new CopybookManager(copybookRepositoryMock, 
                TestHelper.ConfigurationSettingsMock, TestHelper.FileOperationsManagerMock);
            copybookManager.DeleteCopybookFileAssociation("COPYBOOK1", "EBCDICFILE1");

            copybookRepositoryMock.VerifyAllExpectations();
        }

        [TestMethod]
        public void Does_CopybookManager_Get_All_Copybooks()
        {
            var copybookRepositoryMock = TestHelper.CopybookRepositoryMock;
            copybookRepositoryMock.Expect(m => m.GetCopybooks());

            ICopybookManager copybookManager = new CopybookManager(copybookRepositoryMock, 
                TestHelper.ConfigurationSettingsMock, TestHelper.FileOperationsManagerMock);

            copybookManager.GetCopybooks();

            copybookRepositoryMock.VerifyAllExpectations();
        }

        [TestMethod]
        public void Does_CopybookManager_Add_Already_Existing_Association()
        {
            var ebcdicFileName = "EBCDICFILE1";

            var copybookRepositoryMock = TestHelper.CopybookRepositoryMock;
            copybookRepositoryMock.Expect(m => m.GetCopybookPathForEbcdicFile(ebcdicFileName)).Return(new OperationResult<string>
            {
                Result = "COPYBOOK1"
            });

            ICopybookManager copybookManager = new CopybookManager(copybookRepositoryMock, 
                TestHelper.ConfigurationSettingsMock, TestHelper.FileOperationsManagerMock);

            var result = copybookManager.AddCopybookEbcdicFileAssociation("COPYBOOK1", ebcdicFileName);

            copybookRepositoryMock.VerifyAllExpectations();
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Messages);
            Assert.AreEqual(1, result.Messages.Count);
        }

        [TestMethod]
        public void Does_CopybookManager_Handle_Failed_FileSystemm_Operation_On_Add()
        {
            const string ebcdicFileName = "EBCDICFILE1";

            var copybookRepositoryMock = TestHelper.CopybookRepositoryMock;
            copybookRepositoryMock.Expect(m => m.GetCopybookPathForEbcdicFile(ebcdicFileName)).Return(new OperationResult<string>());

            var fileOperationsManager = TestHelper.FileOperationsManagerMock;
            fileOperationsManager.Expect(m => m.CopyFile(Arg<string>.Is.Anything, Arg<string>.Is.Anything)).Return(new OperationResult<string>()
            {
                Result = null,
                Messages = new List<string>() { "Some error message" }
            });

            ICopybookManager copybookManager = new CopybookManager(copybookRepositoryMock, 
                TestHelper.ConfigurationSettingsMock, fileOperationsManager);

            var result = copybookManager.AddCopybookEbcdicFileAssociation("COPYBOOK1", ebcdicFileName);

            copybookRepositoryMock.VerifyAllExpectations();
            fileOperationsManager.VerifyAllExpectations();
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Messages);
            Assert.AreEqual(1, result.Messages.Count);
        }

        [TestMethod]
        public void Does_CopybookManager_Successfully_Add_Association()
        {
            const string ebcdicFileName = "EBCDICFILE1";

            var copybookRepositoryMock = TestHelper.CopybookRepositoryMock;
            copybookRepositoryMock.Expect(m => m.GetCopybookPathForEbcdicFile(ebcdicFileName)).Return(new OperationResult<string>());
            copybookRepositoryMock.Expect(
                m => m.AddCopybookEbcdicFileAssociation(Arg<string>.Is.Anything, Arg<string>.Is.Anything))
                .Return(new OperationResult<bool>()
                {
                    Result = true
                });

            var fileOperationsManager = TestHelper.FileOperationsManagerMock;
            fileOperationsManager.Expect(m => m.CopyFile(Arg<string>.Is.Anything, Arg<string>.Is.Anything)).Return(new OperationResult<string>()
            {
                Result = @"C:\New\CopybookFilePath\COPYBOOK1.fileformat"
            });

            ICopybookManager copybookManager = new CopybookManager(copybookRepositoryMock, 
                TestHelper.ConfigurationSettingsMock, fileOperationsManager);

            var result = copybookManager.AddCopybookEbcdicFileAssociation("COPYBOOK1.fileformat", ebcdicFileName);

            copybookRepositoryMock.VerifyAllExpectations();
            fileOperationsManager.VerifyAllExpectations();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Result);
        }

        [TestMethod]
        public void Does_CopybookManager_Retrieve_Copybook_For_Ebcdic_File()
        {
            const string ebcdicFileName = @"C:\Files\EBCDICFILE1";

            var copybookRepositoryMock = TestHelper.CopybookRepositoryMock;
            copybookRepositoryMock.Expect(m => m.GetCopybookPathForEbcdicFile("EBCDICFILE1"));

            ICopybookManager copybookManager = new CopybookManager(copybookRepositoryMock, 
                TestHelper.ConfigurationSettingsMock, TestHelper.FileOperationsManagerMock);

            var result = copybookManager.GetCopybookPathForEbcdicFile(ebcdicFileName);

            copybookRepositoryMock.VerifyAllExpectations();
        }

        [TestMethod]
        public void Does_CopybookManager_Handle_Delete_Database_Error()
        {
            const string copybookPath = @"C:\New\CopybookFilePath\COPYBOOK1.fileformat";

            var copybookRepositoryMock = TestHelper.CopybookRepositoryMock;
            copybookRepositoryMock.Expect(m => m.DeleteCopybook(copybookPath)).Return(new OperationResult<bool>
            {
                Result = false,
                Messages = new List<string> { "Some error message" }
            });

            var fileOperationsManagerMock = TestHelper.FileOperationsManagerMock;

            ICopybookManager copybookManager = new CopybookManager(copybookRepositoryMock, 
                TestHelper.ConfigurationSettingsMock, fileOperationsManagerMock);

            var result = copybookManager.DeleteCopybook(copybookPath);

            fileOperationsManagerMock.AssertWasNotCalled(m => m.DeleteFile(copybookPath));
            copybookRepositoryMock.VerifyAllExpectations();
        }

        [TestMethod]
        public void Does_CopybookManager_Handle_Delete_Copybook_File_Error()
        {
            const string copybookPath = @"C:\New\CopybookFilePath\COPYBOOK1.fileformat";

            var copybookRepositoryMock = TestHelper.CopybookRepositoryMock;
            copybookRepositoryMock.Expect(m => m.DeleteCopybook(copybookPath)).Return(new OperationResult<bool>
            {
                Result = true
            });

            var fileOperationsManagerMock = TestHelper.FileOperationsManagerMock;
            fileOperationsManagerMock.Expect(m => m.DeleteFile(copybookPath)).Return(new OperationResult<bool>
            {
                Result = false,
                Messages = new List<string> {"Some Error"}
            });

            ICopybookManager copybookManager = new CopybookManager(copybookRepositoryMock, 
                TestHelper.ConfigurationSettingsMock, fileOperationsManagerMock);

            var result = copybookManager.DeleteCopybook(copybookPath);

            copybookRepositoryMock.VerifyAllExpectations();
            fileOperationsManagerMock.VerifyAllExpectations();

            Assert.IsNotNull(result);
            Assert.IsFalse(result.Result);
            Assert.AreEqual(1, result.Messages.Count);
        }

        [TestMethod]
        public void Can_CopybookManager_Successfully_Delete_Copybook()
        {
            const string copybookPath = @"C:\New\CopybookFilePath\COPYBOOK1.fileformat";

            var copybookRepositoryMock = TestHelper.CopybookRepositoryMock;
            copybookRepositoryMock.Expect(m => m.DeleteCopybook(copybookPath)).Return(new OperationResult<bool>
            {
                Result = true
            });

            var fileOperationsManagerMock = TestHelper.FileOperationsManagerMock;
            fileOperationsManagerMock.Expect(m => m.DeleteFile(copybookPath)).Return(new OperationResult<bool>
            {
                Result = true
            });

            ICopybookManager copybookManager = new CopybookManager(copybookRepositoryMock, 
                TestHelper.ConfigurationSettingsMock, fileOperationsManagerMock);

            var result = copybookManager.DeleteCopybook(copybookPath);

            copybookRepositoryMock.VerifyAllExpectations();
            fileOperationsManagerMock.VerifyAllExpectations();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Result);
            Assert.AreEqual(0, result.Messages.Count);
        }
    }
}
