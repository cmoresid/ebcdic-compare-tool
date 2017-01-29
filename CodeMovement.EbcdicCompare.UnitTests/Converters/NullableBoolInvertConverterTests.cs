using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CodeMovement.EbcdicCompare.Presentation.Converter;

namespace CodeMovement.EbcdicCompare.UnitTests.Converters
{
    [TestClass]
    public class NullableBoolInvertConverterTests
    {
        [TestMethod]
        public void NullableBoolInverterConverter_Convert_Null()
        {
            var converter = new NullableBoolInvertConverter();
            var result = converter.Convert(null, null, null, null);

            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void NullableBoolInverterConverter_Convert_True()
        {
            var converter = new NullableBoolInvertConverter();
            var result = converter.Convert(true, null, null, null);

            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void NullableBoolInverterConverter_Convert_False()
        {
            var converter = new NullableBoolInvertConverter();
            var result = converter.Convert(false, null, null, null);

            Assert.AreEqual(true, result);
        }
    }
}
