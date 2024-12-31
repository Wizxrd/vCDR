using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using vCDR.src;

namespace vCDR
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static async Task DownloadFile(string fileUrl, string downloadPath)
        {
            using (HttpClient client = new HttpClient())
            {
                var fileBytes = await client.GetByteArrayAsync(fileUrl);
                File.WriteAllBytes(downloadPath, fileBytes);
            }
        }

        protected async override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            SplashScreen splashWindow = new SplashScreen();
            splashWindow.TextBlockLoading.Text = "Initializing Database";
            splashWindow.Show();
            await Task.Delay(500);

            splashWindow.TextBlockLoading.Text = "Starting database download";
            string fileUrl = "https://www.fly.faa.gov/rmt/data_file/codedswap_db.csv";
            string downloadPath = Loader.LoadFile("Database", "codedswap_db.csv");
            await DownloadFile(fileUrl, downloadPath);
            splashWindow.TextBlockLoading.Text = "Database download completed";
            await Task.Delay(500);

            splashWindow.TextBlockLoading.Text = "Launching vCDR";
            await Task.Delay(250);
            var mainWindow = new MainWindow();
            splashWindow.Hide();
            await Task.Delay(10);
            mainWindow.Show();
            splashWindow.Close();
        }
    }
}
