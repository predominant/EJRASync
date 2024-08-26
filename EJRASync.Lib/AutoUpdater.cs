using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace EJRASync.Lib
{
    public class AutoUpdater
    {
        private string _exePath;

        public AutoUpdater(string exePath)
        {
            this._exePath = exePath;
        }

        public async Task ProcessUpdates()
        {
            Console.WriteLine($"Running from {this._exePath}");
            Console.WriteLine("Checking for updates...");

            if (await this.UpdateAvailable())
            {
                this.RenameExecutable();
                await this.DownloadUpdate();
            }
            else
            {
                Console.WriteLine("No updates available.");
            }
        }

        private async Task<bool> UpdateAvailable()
        {
            // Get the JSON from the GitHub API
            // Parse the JSON
            // Compare the version number

            var url = Constants.GithubReleaseURL;
            var client = new HttpClient();
            var response = await client.GetAsync(url);
            var json = await response.Content.ReadAsStringAsync();

            return true;
        }

        private void RenameExecutable()
        {
            File.Move(this._exePath, $"{this._exePath}.OLD");
        }

        private async Task DownloadUpdate()
        {
            
        }
    }
}
