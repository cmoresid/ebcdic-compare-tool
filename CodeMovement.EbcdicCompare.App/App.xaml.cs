using System.Windows;

namespace CodeMovement.EbcdicCompare.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var bootstrapper = new Bootstrapper();
            bootstrapper.Run();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            // Ensure that all SQLiteConnections are closed.
            System.Data.SQLite.SQLiteConnection.ClearAllPools();
        }
    }
}
