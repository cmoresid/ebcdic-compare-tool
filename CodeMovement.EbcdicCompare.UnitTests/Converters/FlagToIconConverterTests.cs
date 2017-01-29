using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CodeMovement.EbcdicCompare.Presentation.Converter;
using CodeMovement.EbcdicCompare.Models;

namespace CodeMovement.EbcdicCompare.UnitTests.Converters
{
    [TestClass]
    public class FlagToIconConverterTests
    {
        [TestMethod]
        public void FlagToIconConverter_Different_To_Icon()
        {
            var converter = new FlagToIconConverter();
            var result = converter.Convert(RecordFlag.Different, null, null, null);

            Assert.AreEqual("pack://application:,,,/Images/red.png", result);
        }

        [TestMethod]
        public void FlagToIconConverter_Identical_To_Icon()
        {
            var converter = new FlagToIconConverter();
            var result = converter.Convert(RecordFlag.Identical, null, null, null);

            Assert.AreEqual("pack://application:,,,/Images/green.png", result);
        }

        [TestMethod]
        public void FlagToIconConverter_Extra_To_Icon()
        {
            var converter = new FlagToIconConverter();
            var result = converter.Convert(RecordFlag.Extra, null, null, null);

            Assert.AreEqual("pack://application:,,,/Images/extra.png", result);
        }

        [TestMethod]
        public void FlagToIconConverter_None_To_Icon()
        {
            var converter = new FlagToIconConverter();
            var result = converter.Convert(RecordFlag.None, null, null, null);

            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void FlagToIconConverter_Convert_Back()
        {
            var converter = new FlagToIconConverter();
            var result = converter.ConvertBack(null, null, null, null);

            Assert.AreEqual(null, result);
        }
    }
}
