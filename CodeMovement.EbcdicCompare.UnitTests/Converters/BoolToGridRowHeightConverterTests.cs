using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CodeMovement.EbcdicCompare.Presentation.Converter;
using System.Globalization;
using System.Windows;

namespace CodeMovement.EbcdicCompare.UnitTests.Converters
{
    [TestClass]
    public class BoolToGridRowHeightConverterTests
    {
        [TestMethod]
        public void BoolToGridRowHeightConverter_Convert_True_To_Grid_Height()
        {
            var converter = new BoolToGridRowHeightConverter();
            var gridHeight = converter.Convert(true, null, "10", CultureInfo.InvariantCulture);

            Assert.AreEqual(new GridLength(10, GridUnitType.Pixel), gridHeight);
        }

        [TestMethod]
        public void BoolToGridRowHeightConverter_Convert_False_To_0_Grid_Height()
        {
            var converter = new BoolToGridRowHeightConverter();
            var gridHeight = converter.Convert(false, null, "10", CultureInfo.InvariantCulture);

            Assert.AreEqual(new GridLength(0, GridUnitType.Pixel), gridHeight);
        }

        [TestMethod]
        public void BoolToGridRowHeightConverter_Convert_Back()
        {
            var converter = new BoolToGridRowHeightConverter();
            var result = converter.ConvertBack(true, null, null, null);

            Assert.IsNull(result);
        }
    }
}
