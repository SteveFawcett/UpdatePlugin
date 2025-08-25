using BroadcastPluginSDK.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;


namespace UpdatePlugin.Classes
{
    static class Downloader
    {
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> _installLocks = new();

        public static async Task Install(IConfiguration config, ILogger<IPlugin> logger, ReleaseListItem selected)
        {
            var installPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) ,  "broadcast" , "plugins"  );
            var semaphore = _installLocks.GetOrAdd(selected.ShortName, _ => new SemaphoreSlim(1, 1));
            await semaphore.WaitAsync();

            try
            {
                logger.LogInformation("Starting download of plugin: {ShortName} ", selected.ShortName);
                var result = await DownloadAndInstall(logger, selected.DownloadUrl);
                logger.LogInformation("Download completed for plugin: {ShortName}", selected.ShortName);

                logger.LogInformation(result != null
                    ? "Verifying installation for plugin: {ShortName} at {InstallPath}"
                    : "Download failed for plugin: {ShortName}", selected.ShortName, result);

                if (result == null) return;

                var installFile = Path.Combine(installPath, selected.ShortName + ".zip");

                logger.LogInformation("Copying {result} to {destination}", result, installFile);

                if (File.Exists(installFile))
                {
                    logger.LogInformation("Existing file {installFile} will be deleted" , installFile);
                    File.Delete(installFile);
                }

                File.Move(result, installFile);

                logger.LogInformation("Installation process completed for plugin: {ShortName}", selected.ShortName);
            }
            finally
            {
                semaphore.Release();
                _installLocks.TryRemove(selected.ShortName, out _);
            }
        }


        async static Task<string?> DownloadAndInstall(ILogger<IPlugin> log, string url)
        {
            string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() );
            Directory.CreateDirectory(tempPath);
            string destinationPath = Path.Combine(tempPath, "plugin.tmp");

            log.LogInformation("📥 Downloading from {Url} to {DestinationPath}", url, destinationPath);
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            try
            {
                var dir = Path.GetDirectoryName(destinationPath)!;
                Directory.CreateDirectory(dir);

                if (File.Exists(destinationPath))
                {
                    File.SetAttributes(destinationPath, FileAttributes.Normal);
                    File.Delete(destinationPath);
                }

                await using var stream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None);
                await response.Content.CopyToAsync(stream);

                log.LogInformation("✅ Plugin downloaded to {DestinationPath}", destinationPath);
            }
            catch (UnauthorizedAccessException ex)
            {
                log.LogError(ex, "❌ Access denied when writing to {DestinationPath}", destinationPath);
                MessageBox.Show($"Access denied when writing to {destinationPath}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            catch (IOException ex)
            {
                log.LogError(ex.Message, "⚠️ {DestinationPath}", destinationPath);
                return null;
            }
            catch (Exception ex)
            {
                log.LogError(ex, "⚠️ Unexpected error during download to {DestinationPath}", destinationPath);
                return null;
            }

            return destinationPath;
        }

        static bool CheckExistingInstallation(ILogger<IPlugin> log, string destinationPath)
        {
            log.LogInformation("Checking existing installation at {DestinationPath}", destinationPath);

            string? parentDirectory = Path.GetDirectoryName(destinationPath);
            if (string.IsNullOrWhiteSpace(parentDirectory))
            {
                log.LogError("Invalid destination path: {DestinationPath}", destinationPath);
                return false;
            }

            if (!Directory.Exists(parentDirectory))
            {
                log.LogInformation("Creating missing directory: {ParentDirectory}", parentDirectory);
                Directory.CreateDirectory(parentDirectory);
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
