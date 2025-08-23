using Broadcast.Classes;
using BroadcastPluginSDK.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using UpdatePlugin.Classes;

namespace BroadcastPluginSDK.Classes
{
    public class PluginUpdater : IPluginUpdater
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

            var repository = _config.GetValue<string>("RepositoryUrl") ?? jsonUrl;
            _logger.LogInformation("Starting Plugin Updater {0}", repository);

            _logger.LogDebug( "Currently loaded plugins is {0}", _registry.GetAll().Count);

            foreach ( IPlugin x in _registry.GetAll())
            {
                _logger.LogDebug("Registered plugin: {0}", x.Name);
            }

            _ = Task.Run(async () =>
            {
                _releases = await GetReleases(repository);
                foreach (var release in _releases)
                {
                    if (string.IsNullOrWhiteSpace(release.ReadMe))
                    {
                        release.ShortName = release.Repo.Split('/').Last() ?? release.Repo;

                        _logger.LogDebug("Fetching README for {0}", release.ShortName);
                        
                        release.ReadMe = await GetReadme(release.ReadMeUrl);
                        
                        IPlugin? found = _registry.GetAll().FirstOrDefault(p =>
                            p.ShortName.Equals(release.ShortName, StringComparison.OrdinalIgnoreCase));
                        
                        if (found != null)
                        {
                            release.Installed = GetInstalled(_registry, release.ShortName);
                            release.IsLatest = release.Version.Equals(found.Version, StringComparison.OrdinalIgnoreCase);
                        }
                        else
                        {
                            release.Installed = string.Empty;
                            release.IsLatest = false;
                        }

                    }
                }
            });
        }
        public List<string> Versions(string shortName)
        {
            return Releases
                .Where(r => string.Equals(r.ShortName, shortName, StringComparison.OrdinalIgnoreCase))
                .Select(r => r.Version)
                .Distinct() // Optional: remove duplicates
                .OrderByDescending(v => Version.Parse(v)) // Optional: sort by version descending
                .ToList();
        }

        public List<ReleaseListItem> Latest()
        {
            return Releases
                .GroupBy(r => r.ShortName)
                .Select(g => g
                    .OrderByDescending(r => Version.Parse(r.Version))
                    .First())
                .ToList();
        }

        static string GetInstalled( IPluginRegistry list , string ShortName)
        {
            var current = list.GetAll()
                .FirstOrDefault(p => p.ShortName.Equals(ShortName, StringComparison.OrdinalIgnoreCase))?.Version;

            if (string.IsNullOrWhiteSpace(current))
            {
                return string.Empty;
            } 

            return string.Join(".", current.Split('.').Take(3));

        }
        private async Task<ReleaseListItem[]> GetReleases(string jsonUrl)
        {
            try
            {
                var service = await ReleaseService.CreateAsync(jsonUrl);
                return service.GetReleaseItems().ToArray();
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
                string markdown = await DownloadStringAsync(url, _logger ) ?? "# README Not found";
                return markdown;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching README: {ex.Message}");
                return string.Empty;
            }
        }


        public static async Task<string?> DownloadStringAsync(string url, ILogger<IPlugin>? _logger = null, CancellationToken cancellationToken = default)
        {
            try
            {
                if (_logger != null)
                {
                    _logger.LogInformation("📥 Downloading from URL: {Url}", url);
                }
                else
                {
                    Console.WriteLine($"📥 Downloading from URL: {url}");
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
