using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CodeMovement.EbcdicCompare.DataAccess;

namespace CodeMovement.EbcdicCompare.UnitTests.DataAccess
{
    [TestClass]
    public class ConfigurationSettingsTests
    {
        [TestMethod]
        public void ConfigurationSettings_Properties()
        {
            var configurationSettings = new ConfigurationSettings();

            Assert.AreEqual(@"Data Source=.\Copybook.db;Version=3", configurationSettings.CopybookDbConnectionString);
            Assert.AreEqual(@"C:\Users\c.moreside\Db\Copybooks", configurationSettings.CopybookFolderPath);
            Assert.AreEqual(@"C:\Program Files (x86)\Textplorer\TEP.exe", configurationSettings.ExternalEditorPath);
        }
    }
}
