using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CodeMovement.EbcdicCompare.Presentation.Converter;
using System.Globalization;
using System.Windows;

namespace CodeMovement.EbcdicCompare.UnitTests.Converters
{
    [TestClass]
    public class BooleanToVisibilityConverterTests
    {
        [TestMethod]
        public void BooleanToVisibilityConverter_Convert_True_To_Visible()
        {
            var converter = new BooleanToVisibilityConverter();
            var result = converter.Convert(true, null, null, CultureInfo.InvariantCulture);

            Assert.AreEqual(Visibility.Visible, result);
        }

        [TestMethod]
        public void BooleanToVisibilityConverter_Convert_False_To_Collapsed()
        {
            var converter = new BooleanToVisibilityConverter();
            var result = converter.Convert(false, null, null, CultureInfo.InvariantCulture);

            Assert.AreEqual(Visibility.Collapsed, result);
        }

        [TestMethod]
        public void BooleanToVisibilityConverter_Convert_Back_Visible()
        {
            var converter = new BooleanToVisibilityConverter();
            var result = converter.ConvertBack(Visibility.Visible, null, null, CultureInfo.InvariantCulture);

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void BooleanToVisibilityConverter_Convert_Back_Collapsed()
        {
            var converter = new BooleanToVisibilityConverter();
            var result = converter.ConvertBack(Visibility.Collapsed, null, null, CultureInfo.InvariantCulture);

            Assert.AreEqual(false, result);
        }
    }
}
