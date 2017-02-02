namespace CodeMovement.EbcdicCompare.DataAccess
{
    public interface IConfigurationSettings
    {
        string CopybookDbConnectionString { get; }
        string CopybookFolderPath { get; }
        string ExternalEditorPath { get; }
    }
}
