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
            var installPath = config["PluginInstallPath"] ?? string.Empty;
            if(string.IsNullOrWhiteSpace(installPath))
            {
                installPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Broadcast\plugins";
                logger.LogError("PluginInstallPath is not configured. setting to {path}" , installPath );
                config["PluginInstallPath"] = installPath;
            }

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

                DeleteFiles(selected.ShortName, installPath);

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

        static void DeleteFiles(string shortName , string destinationPath )
        {
            if (string.IsNullOrWhiteSpace(shortName) || string.IsNullOrWhiteSpace(destinationPath))
                return;

            string directory = Path.GetDirectoryName(destinationPath) ?? string.Empty;
            if (string.IsNullOrWhiteSpace(directory) || !Directory.Exists(directory))
                return;

            string pattern = $"{shortName}-v*.zip";

            foreach (string file in Directory.GetFiles(directory, pattern))
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }
        }
    }
}
