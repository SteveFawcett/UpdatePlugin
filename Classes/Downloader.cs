using BroadcastPluginSDK.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdatePlugin.Classes
{
    static class Downloader
    {
        public static async void Install(IConfiguration config, ILogger<IPlugin> logger, ReleaseListItem selected)
        {
            string installPath = config["plugindirectory"] ?? throw new ArgumentNullException("plugindirectory not set in configuration");
            if (CheckExistingInstallation(logger, installPath) == false)
            {
                logger.LogError("Installation aborted due to existing read-only file at {InstallPath}", installPath);
                return;
            }

            logger.LogInformation("Starting installation of plugin: {PluginName} into {installPath}", selected.ShortName, installPath);
            await DownloadAndInstall(logger, selected.DownloadUrl, installPath);
        }

        public static async void Update(IConfiguration config, ILogger<IPlugin> logger, ReleaseListItem selected, ReleaseListItem current)
        {
            string installPath = config["plugindirectory"] ?? throw new ArgumentNullException("plugindirectory not set in configuration");
            if (CheckExistingInstallation(logger, installPath) == false)
            {
                logger.LogError("Installation aborted due to existing read-only file at {InstallPath}", installPath);
                return;
            }

            logger.LogInformation("Current version of plugin: {PluginName} is {CurrentVersion}", selected.ShortName, current.Version);

            logger.LogInformation("Starting update of plugin: {PluginName} in {installPath}", selected.ShortName, installPath);
            await DownloadAndInstall(logger, selected.DownloadUrl, installPath);
        }

        async static Task DownloadAndInstall(ILogger<IPlugin> log, string url, string destinationPath)
        {
            log.LogInformation("Downloading from {Url} to {DestinationPath}", url, destinationPath);
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            try
            {
                await using var fileStream = System.IO.File.Create(destinationPath);
                await response.Content.CopyToAsync(fileStream);
            }
            catch (UnauthorizedAccessException)
            {
                log.LogError("Access denied when writing to {DestinationPath}", destinationPath);
                MessageBox.Show($"Access denied when writing to {destinationPath} " ,  "Error" , MessageBoxButtons.OK, MessageBoxIcon.Error );
            }

            catch (Exception ex)
            {
                log.LogError(ex, "Unexpected error during download to {DestinationPath}", destinationPath);
            }

        }

        static bool CheckExistingInstallation(ILogger<IPlugin> log, string destinationPath)
        {
            log.LogInformation("Checking existing installation at {DestinationPath}", destinationPath);
            if (!Directory.Exists(destinationPath))
            {
                log.LogInformation("Directory does not exist at {DestinationPath}", destinationPath);
                return false;
            }

            if (File.Exists(destinationPath))
            {
                log.LogInformation("File already exists at {DestinationPath}", destinationPath);
                var attributes = File.GetAttributes(destinationPath);
                if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    log.LogWarning("File at {DestinationPath} is read-only.", destinationPath);
                    return false;
                }
            }

            return true;
        }
    }
}
