using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CodeMovement.EbcdicCompare.Models;
using CodeMovement.EbcdicCompare.Models.Copybook;
using CodeMovement.EbcdicCompare.Models.Ebcdic;
using CodeMovement.EbcdicCompare.Models.ViewModel;

namespace CodeMovement.EbcdicCompare.Tests.Models
{
    [TestClass]
    public class EbcdicFileRecordModelTests
    {
        #region "Test Data"

        private static readonly Dictionary<string, FieldFormat> _fieldFormats = new Dictionary<string, FieldFormat>
        {
            { "9_+0000.00", new FieldFormat { Name = "9_+0000.00", Size = "6", Type = "9", Signed = true, Decimal = 2, ImpliedDecimal = false } },
            { "9_0000.00", new FieldFormat { Name = "9_0000.00", Size = "6", Type = "9" , Signed = false, Decimal = 2, ImpliedDecimal = false } },
            { "9_0000", new FieldFormat { Name = "9_0000", Size = "4", Type = "9", Signed = false, Decimal = 0, ImpliedDecimal = false } },
            { "3_+0000.00", new FieldFormat { Name = "3_+0000.00", Size = "6", Type = "3", Signed = true, Decimal = 2, ImpliedDecimal = false } },
            { "3_0000.00", new FieldFormat { Name = "3_0000.00", Size = "6", Type = "3" , Signed = false, Decimal = 2, ImpliedDecimal = false } },
            { "3_0000", new FieldFormat { Name = "3_0000", Size = "4", Type = "3", Signed = false, Decimal = 0, ImpliedDecimal = false } },
            { "X_aaa", new FieldFormat { Name = "X_aaa", Size = "3", Type = "X" } },
        };

        public Dictionary<string, FieldFormat> FieldFormats
        {
            get { return _fieldFormats; }
        }

        public EbcdicFileRow CreateRow(string recordName, List<FieldValuePair> pairs)
        {
            return new EbcdicFileRow
            {
                RecordTypeName = recordName,
                FieldValues = pairs
            };
        }

        public EbcdicFileRow CreateRow(string recordName, string fieldFormat, object value)
        {
            return new EbcdicFileRow
            {
                RecordTypeName = recordName,
                FieldValues = new List<FieldValuePair> { new FieldValuePair(FieldFormats[fieldFormat], value) }
            };
        }

        private EbcdicFileRecordModel CreateEbcdicFileRecordModel(string fieldFormat, object value)
        {
            return CreateEbcdicFileRecordModel("RECORD", fieldFormat, value);
        }

        private EbcdicFileRecordModel CreateEbcdicFileRecordModel(string recordName, string fieldFormat, object value)
        {
            var fieldValuePairs = new List<FieldValuePair>
            {
                new FieldValuePair(FieldFormats[fieldFormat], value)
            };

            return new EbcdicFileRecordModel(CreateRow(recordName, fieldValuePairs));
        }

        #endregion

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EbcdicFileRecordModel_Constructor_Null_Test()
        {
            var ebcdicFileRecordModel = new EbcdicFileRecordModel(null);
        }

        [TestMethod]
        public void EbcdicFileRecordModel_Constructor_Valid_Value_Test()
        {
            var viewModel = new EbcdicFileRecordModel(CreateRow("RECORD", "9_+0000.00", 10.53m));
            
            Assert.AreEqual(string.Empty, viewModel.ColumnHeading);
            Assert.AreEqual(RecordFlag.None, viewModel.Flag);
            Assert.AreEqual("RECORD", viewModel.RecordTypeName);
            Assert.AreEqual("+0010.53  ", viewModel.RowValue);
            Assert.AreEqual(1, viewModel.RowNumber);
        }

        [TestMethod]
        public void EbcdicFileRecordModel_PositiveZoned_Format()
        {
            var viewModel = CreateEbcdicFileRecordModel("9_+0000.00", 9923.30m);
            Assert.AreEqual("+9923.30  ", viewModel.RowValue);
        }

        [TestMethod]
        public void EbcdicFileRecordModel_NegativeZoned_Format()
        {
            var viewModel = CreateEbcdicFileRecordModel("9_+0000.00", -8.99m);
            Assert.AreEqual("-0008.99  ", viewModel.RowValue);
        }

        [TestMethod]
        public void EbcdicFileRecordModel_UnsignedZoned_WithNegativeDecimal_Format()
        {
            var viewModel = CreateEbcdicFileRecordModel("9_0000.00", -20.99m);
            Assert.AreEqual("0020.99  ", viewModel.RowValue);
        }

        [TestMethod]
        public void EbcdicFileRecordModel_UnsignedZoned_WithPositiveDecimal_Format()
        {
            var viewModel = CreateEbcdicFileRecordModel("9_0000.00", 888.1m);
            Assert.AreEqual("0888.10  ", viewModel.RowValue);
        }

        [TestMethod]
        public void EbcdicFileRecordModel_UnsignedZoned_NoDecimals_Format()
        {
            var viewModel = CreateEbcdicFileRecordModel("9_0000", 1234.20m);
            Assert.AreEqual("1234  ", viewModel.RowValue);
        }

        [TestMethod]
        public void EbcdicFileRecordModel_PositivePacked_Format()
        {
            var viewModel = CreateEbcdicFileRecordModel("3_+0000.00", 9923.30m);
            Assert.AreEqual("+9923.30  ", viewModel.RowValue);
        }

        [TestMethod]
        public void EbcdicFileRecordModel_NegativePacked_Format()
        {
            var viewModel = CreateEbcdicFileRecordModel("3_+0000.00", -8.99m);
            Assert.AreEqual("-0008.99  ", viewModel.RowValue);
        }

        [TestMethod]
        public void EbcdicFileRecordModel_UnsignedPacked_WithNegativeDecimal_Format()
        {
            var viewModel = CreateEbcdicFileRecordModel("3_0000.00", -20.99m);
            Assert.AreEqual("0020.99  ", viewModel.RowValue);
        }

        [TestMethod]
        public void EbcdicFileRecordModel_UnsignedPacked_WithPositiveDecimal_Format()
        {
            var viewModel = CreateEbcdicFileRecordModel("3_0000.00", 888.1m);
            Assert.AreEqual("0888.10  ", viewModel.RowValue);
        }

        [TestMethod]
        public void EbcdicFileRecordModel_UnsignedPacked_NoDecimals_Format()
        {
            var viewModel = CreateEbcdicFileRecordModel("3_0000", 1234.20m);
            Assert.AreEqual("1234  ", viewModel.RowValue);
        }

        [TestMethod]
        public void EbcdicFileRecordModel_AlphaNumeric_Format()
        {
            var viewModel = CreateEbcdicFileRecordModel("X_aaa", "abcd");
            Assert.AreEqual("abcd ", viewModel.RowValue);
        }

        [TestMethod]
        public void EbcdicFileRecordModel_AlphNumeric_Longer_Than_Field_Name_Format()
        {
            var viewModel = CreateEbcdicFileRecordModel("X_aaa", "abcdefghijklmnop");
            Assert.AreEqual("abcdefghijklmnop", viewModel.RowValue);
        }

        [TestMethod]
        public void EbcdicFileRecordModel_Test_Equal_Objects()
        {
            var viewModel1 = CreateEbcdicFileRecordModel("X_aaa", "abc");
            var viewModel2 = CreateEbcdicFileRecordModel("X_aaa", "abc");

            Assert.IsTrue(viewModel1.Equals(viewModel2));
        }

        [TestMethod]
        public void EbcdicFileRecordModel_Test_Not_Equal_Objects()
        {
            var viewModel1 = CreateEbcdicFileRecordModel("X_aaa", "abc");
            var viewModel2 = CreateEbcdicFileRecordModel("X_aaa", "abcdefg");

            Assert.IsFalse(viewModel1.Equals(viewModel2));
        }

        [TestMethod]
        public void EbcdicFileRecordModel_Multiple_Fields_In_Record_Values()
        {
            var fileRow = new EbcdicFileRow
            {
                RecordTypeName = "RECORD",
                FieldValues = new List<FieldValuePair>
                {
                    new FieldValuePair(FieldFormats["X_aaa"], "ab"),
                    new FieldValuePair(FieldFormats["3_0000.00"], 412.2m)
                }
            };

            var viewModel = new EbcdicFileRecordModel(fileRow);

            Assert.AreEqual("ab    0412.20  ", viewModel.RowValue);
        }

        [TestMethod]
        public void EbcdicFileRecordModel_Format_Header_One_Field()
        {
            var viewModel = CreateEbcdicFileRecordModel("X_aaa", "abc");
            viewModel.PopulateColumnHeading();

            Assert.AreEqual("X_aaa", viewModel.ColumnHeading);
        }

        [TestMethod]
        public void EbcdicFileRecordModel_Format_Header_Two_Fields()
        {
            var fileRow = new EbcdicFileRow
            {
                RecordTypeName = "RECORD",
                FieldValues = new List<FieldValuePair>
                {
                    new FieldValuePair(FieldFormats["X_aaa"], "ab"),
                    new FieldValuePair(FieldFormats["3_0000.00"], 412.2m)
                }
            };

            var viewModel = new EbcdicFileRecordModel(fileRow);
            viewModel.PopulateColumnHeading();

            Assert.AreEqual("X_aaa 3_0000.00", viewModel.ColumnHeading);
        }

        [TestMethod]
        public void EbcdicFileRecordModel_Check_Multiple_Same_Type_Record_Headers()
        {
            var ebcdicFileRows = new List<EbcdicFileRow>
            {
                CreateRow("RECORD_A", "3_0000.00", 4.23m),
                CreateRow("RECORD_A", "3_0000.00", 100.21)
            };

            var viewModels = EbcdicFileRecordModel.Map(ebcdicFileRows);

            Assert.IsNotNull(viewModels);
            Assert.AreEqual(viewModels.Count, 2);

            Assert.AreEqual("3_0000.00", viewModels[0].ColumnHeading);
            Assert.AreEqual(string.Empty, viewModels[1].ColumnHeading);
        }

        [TestMethod]
        public void EbcdicFileRecordModel_Check_Multiple_Different_Types_Record_Headers()
        {
            var ebcdicFileRows = new List<EbcdicFileRow>
            {
                CreateRow("RECORD_A", "3_0000.00", 4.23m),
                CreateRow("RECORD_A", "3_0000.00", 100.21m),
                CreateRow("RECORD_B", "X_aaa", "abcdef")
            };

            var viewModels = EbcdicFileRecordModel.Map(ebcdicFileRows);

            Assert.IsNotNull(viewModels);
            Assert.AreEqual(viewModels.Count, 3);

            Assert.AreEqual("3_0000.00", viewModels[0].ColumnHeading);
            Assert.AreEqual(string.Empty, viewModels[1].ColumnHeading);
            Assert.AreEqual("X_aaa ", viewModels[2].ColumnHeading);
        }
    }
}
