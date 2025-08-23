using Broadcast.Classes;
using BroadcastPluginSDK.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using UpdatePlugin.Classes;
using System.Security.Policy;

namespace UpdatePlugin.Classes
{
    public class PluginUpdater 
    {
        private readonly IPluginRegistry _registry;
        private readonly ILogger<IPlugin> _logger;
        private readonly IConfiguration _config;
        private ReleaseListItem[] _releases;
        private static readonly HttpClient _httpClient = new();

        public ReleaseListItem[] Releases => _releases; 

        private const string jsonUrl = "https://raw.githubusercontent.com/SteveFawcett/delivery/refs/heads/main/releases.json";

        public PluginUpdater(IConfiguration config,  IPluginRegistry registry, ILogger<IPlugin> logger)
        {
            _registry = registry;
            _logger = logger;
            _config = config;   
            _releases = Array.Empty<ReleaseListItem>();


            _logger.LogDebug( "Currently loaded plugins is {0}", _registry.GetAll().Count);

            foreach ( IPlugin x in _registry.GetAll())
            {
                _logger.LogDebug("Registered plugin: {0}", x.Name);
            }

        }

        public async Task<ReleaseListItem[]> GetReleases()
        {

            string j = _config.GetValue<string>("RepositoryUrl") ?? jsonUrl;
            _logger.LogInformation("Starting Plugin Updater {0}", j);

            try
            {
                var service = await ReleaseService.CreateAsync(j);
                var _releases =  service.GetReleaseItems().ToArray();
                foreach (var release in _releases)
                {
                    _logger.LogInformation("Processing {name}", release.ShortName);
                    release.ReadMe = await  GetReadme(release.ReadMeUrl);
                }
                return _releases;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch releases");
                return Array.Empty<ReleaseListItem>();
            }
        }

        private async Task<string> GetReadme(string url)
        {
            try
            {
                // pass the logger to the DownloadStringAsync method
                string markdown = await DownloadStringAsync(url ) ?? "# README Not found";
                return markdown;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching README: {ex.Message}");
                return string.Empty;
            }
        }


        public  async Task<string?> DownloadStringAsync( string url, CancellationToken cancellationToken = default)
        {
            try
            {
                if (_logger != null)
                {
                    _logger.LogInformation("📥 Downloading README from URL: {Url}", url);
                }
                else
                {
                    Console.WriteLine($"📥 Downloading README from URL: {url}");
                }

                var response = await _httpClient.GetAsync(url, cancellationToken);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                var message = $"❌ Error downloading string: {ex.Message}";
                if (_logger != null)
                {
                    _logger.LogError("❌ Error downloading string: {ErrorMessage}", ex.Message);
                }
                else
                {
                    Console.WriteLine(message);
                }

                return null;
            }
        }


    }
}
