using Microsoft.VisualStudio.TestTools.UnitTesting;
using CodeMovement.EbcdicCompare.Presentation.Converter;

namespace CodeMovement.EbcdicCompare.UnitTests.Converters
{
    [TestClass]
    public class NullableBoolConverterTests
    {
        [TestMethod]
        public void NullableBoolConverter_Convert_Null()
        {
            var converter = new NullableBoolConverter();
            var result = converter.Convert(null, null, null, null);

            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void NullableBoolConverter_Convert_True()
        {
            var converter = new NullableBoolConverter();
            var result = converter.Convert(true, null, null, null);

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void NullableBoolConverter_Convert_False()
        {
            var converter = new NullableBoolConverter();
            var result = converter.Convert(false, null, null, null);

            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void NullableBoolConverter_Convert_Back_True()
        {
            var converter = new NullableBoolConverter();
            var result = converter.Convert(true, null, null, null);

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void NullableBoolConverter_Convert_Back_False()
        {
            var converter = new NullableBoolConverter();
            var result = converter.Convert(false, null, null, null);

            Assert.AreEqual(false, result);
        }
    }
}
