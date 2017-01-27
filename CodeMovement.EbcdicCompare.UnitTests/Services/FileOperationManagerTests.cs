using Rhino.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CodeMovement.EbcdicCompare.DataAccess;
using CodeMovement.EbcdicCompare.Services;
using System.IO;

namespace CodeMovement.EbcdicCompare.UnitTests.Services
{
    [TestClass]
    public class FileOperationManagerTests
    {
        protected IFileOperation FileOperationMock { get { return MockRepository.GenerateMock<IFileOperation>(); } }

        [TestMethod]
        public void Does_FileOperationsManager_Read_File_As_String_Properly()
        {
            var fileOperation = FileOperationMock;
            var filePath = @"C:\My\FilePath.txt";

            fileOperation.Expect(m => m.ReadAllLines(filePath)).Return(new string[] { "Line 1", "Line 2", "Line 3" });

            var fileOperationManager = new FileOperationsManager(fileOperation);
            var results = fileOperationManager.ReadFileAsString(filePath);

            Assert.IsNotNull(results);
            Assert.IsTrue(results.Successful);
            Assert.AreEqual("Line 1\nLine 2\nLine 3", results.Result);

            fileOperation.VerifyAllExpectations();
        }

        [TestMethod]
        public void Does_FileOperations_ReadAllLines_Handle_Null_File_Path()
        {
            var fileOperation = FileOperationMock;

            var fileOperationManager = new FileOperationsManager(fileOperation);
            var results = fileOperationManager.ReadFileAsString(null);

            Assert.IsNotNull(results);
            Assert.IsFalse(results.Successful);
            Assert.AreEqual(1, results.Messages.Count);

            fileOperation.AssertWasNotCalled(m => m.ReadAllLines(null));
        }

        [TestMethod]
        public void Does_FileOperationsManager_Handle_Exception_In_Read_File()
        {
            var fileOperation = FileOperationMock;
            var filePath = @"C:\My\FilePath.txt";

            fileOperation.Expect(m => m.ReadAllLines(filePath)).Throw(new FileNotFoundException());

            var fileOperationManager = new FileOperationsManager(fileOperation);
            var results = fileOperationManager.ReadFileAsString(filePath);

            Assert.IsNotNull(results);
            Assert.IsFalse(results.Successful);
            Assert.AreEqual(1, results.Messages.Count);

            fileOperation.VerifyAllExpectations();
        }

        [TestMethod]
        public void Does_FileOperationsManager_Read_File_As_Bytes_Properly()
        {
            var fileOperation = FileOperationMock;
            var filePath = @"C:\My\FilePath.txt";
            var expectedBytes = new byte[] { 1, 2, 3 };

            fileOperation.Expect(m => m.ReadAllBytes(filePath)).Return(expectedBytes);

            var fileOperationManager = new FileOperationsManager(fileOperation);
            var results = fileOperationManager.ReadFileAsBytes(filePath);

            Assert.IsNotNull(results);
            Assert.IsTrue(results.Successful);
            Assert.AreEqual(expectedBytes, results.Result);

            fileOperation.VerifyAllExpectations();
        }

        [TestMethod]
        public void Does_FileOperationsManager_ReadAllBytes_Handle_Null_File_Path()
        {
            var fileOperation = FileOperationMock;

            var fileOperationManager = new FileOperationsManager(fileOperation);
            var results = fileOperationManager.ReadFileAsBytes(null);

            Assert.IsNotNull(results);
            Assert.IsFalse(results.Successful);
            Assert.AreEqual(1, results.Messages.Count);

            fileOperation.AssertWasNotCalled(m => m.ReadAllBytes(null));
        }

        [TestMethod]
        public void Does_FileOperationsManager_Handle_Exception_In_ReadFileAsBytes()
        {
            var fileOperation = FileOperationMock;
            var filePath = @"C:\My\FilePath.txt";

            fileOperation.Expect(m => m.ReadAllBytes(filePath)).Throw(new FileNotFoundException());

            var fileOperationManager = new FileOperationsManager(fileOperation);
            var results = fileOperationManager.ReadFileAsBytes(filePath);

            Assert.IsNotNull(results);
            Assert.IsFalse(results.Successful);
            Assert.AreEqual(1, results.Messages.Count);

            fileOperation.VerifyAllExpectations();
        }

        [TestMethod]
        public void Does_FileOperationsManager_Delete_File_Properly()
        {
            var fileOperation = FileOperationMock;
            var filePath = @"C:\My\FilePath.txt";

            fileOperation.Expect(m => m.Delete(filePath));

            var fileOperationManager = new FileOperationsManager(fileOperation);
            var results = fileOperationManager.DeleteFile(filePath);

            Assert.IsNotNull(results);
            Assert.IsTrue(results.Successful);
            Assert.IsTrue(results.Result);

            fileOperation.VerifyAllExpectations();
        }

        [TestMethod]
        public void Does_FileOperationsManager_Delete_Handle_Null_File_Path()
        {
            var fileOperation = FileOperationMock;

            var fileOperationManager = new FileOperationsManager(fileOperation);
            var results = fileOperationManager.DeleteFile(null);

            Assert.IsNotNull(results);
            Assert.IsFalse(results.Successful);
            Assert.AreEqual(1, results.Messages.Count);

            fileOperation.AssertWasNotCalled(m => m.Delete(null));
        }

        [TestMethod]
        public void Does_FileOperationsManager_Handle_Exception_In_Delete_File()
        {
            var fileOperation = FileOperationMock;
            var filePath = @"C:\My\FilePath.txt";

            fileOperation.Expect(m => m.Delete(filePath)).Throw(new FileNotFoundException());

            var fileOperationManager = new FileOperationsManager(fileOperation);
            var results = fileOperationManager.DeleteFile(filePath);

            Assert.IsNotNull(results);
            Assert.IsFalse(results.Successful);
            Assert.AreEqual(1, results.Messages.Count);

            fileOperation.VerifyAllExpectations();
        }

        [TestMethod]
        public void Does_FileOperationsManager_Handle_Copy_Correctly()
        {
            var fileOperation = FileOperationMock;
            var filePath = @"C:\My\File.txt";
            var destinationPath = @"C:\My\Destination";

            var fileOperationManager = new FileOperationsManager(fileOperation);
            var results = fileOperationManager.CopyFile(filePath, destinationPath);

            Assert.IsNotNull(results);
            Assert.IsTrue(results.Successful);
            Assert.AreEqual(@"C:\My\Destination\File.txt", results.Result);

            fileOperation.AssertWasCalled(m => m.Copy(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<bool>.Is.Anything));
        }

        [TestMethod]
        public void Does_FileOperationsManager_Handle_Null_From_Path_On_Copy()
        {
            var fileOperation = FileOperationMock;
            var destinationPath = @"C:\My\Destination";

            var fileOperationManager = new FileOperationsManager(fileOperation);
            var results = fileOperationManager.CopyFile(null, destinationPath);

            Assert.IsNotNull(results);
            Assert.IsFalse(results.Successful);
            Assert.AreEqual(1, results.Messages.Count);

            fileOperation.AssertWasNotCalled(m => m.Copy(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<bool>.Is.Anything));
        }

        [TestMethod]
        public void Does_FileOperationsManager_Handle_Null_To_Path_On_Copy()
        {
            var fileOperation = FileOperationMock;
            var filePath = @"C:\My\File.txt";

            var fileOperationManager = new FileOperationsManager(fileOperation);
            var results = fileOperationManager.CopyFile(filePath, null);

            Assert.IsNotNull(results);
            Assert.IsFalse(results.Successful);
            Assert.AreEqual(1, results.Messages.Count);

            fileOperation.AssertWasNotCalled(m => m.Copy(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<bool>.Is.Anything));
        }

        [TestMethod]
        public void Does_FileOperationsManager_Handle_Exception_Properly_On_Failed_Copy()
        {
            var filePath = @"C:\My\File.txt";
            var destinationPath = @"C:\My\Destination";

            var fileOperation = FileOperationMock;
            fileOperation.Expect(m => m.Copy(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<bool>.Is.Anything)).Throw(new FileNotFoundException());

            var fileOperationManager = new FileOperationsManager(fileOperation);
            var results = fileOperationManager.CopyFile(filePath, destinationPath);

            Assert.IsNotNull(results);
            Assert.IsFalse(results.Successful);
            Assert.AreEqual(1, results.Messages.Count);

            fileOperation.VerifyAllExpectations();
        }
    }
}
