using System.Configuration;

namespace CodeMovement.EbcdicCompare.DataAccess
{
    public class ConfigurationSettings : IConfigurationSettings
    {
        public string CopybookDbConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["CopybookDb"].ConnectionString;
            }
        }

        public string CopybookFolderPath
        {
            get
            {
                return ConfigurationManager.AppSettings["CopybookLocation"];
            }
        }

        public string ExternalEditorPath
        {
            get
            {
                return ConfigurationManager.AppSettings["ExternalEditor"];
            }
        }
    }
}
