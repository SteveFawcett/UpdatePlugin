namespace UpdatePlugin.Classes;

using BroadcastPluginSDK.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class ReleaseListItem
{
    public string Repo { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Installed { get; set; } = "1.0.0";
    public string ReadMeDocUrl { get; set; } = string.Empty; // This is the URL to the README document, if available
    public string ShortName { get; set; } = string.Empty; // Added for better readability in some contexts
    public bool IsLatest { get; set; } = false;
    public string ZipName { get; set; } = string.Empty;
    public string DownloadUrl { get; set; } = string.Empty;
    public string ReadMeUrl { get; set; } = string.Empty;
    public string ReadMe { get; internal set; } = string.Empty; // This will hold the HTML content of the README

    public override string ToString()
    {
        return $"{Repo} - {Version}";
    }
}
public static class ReleaseListItemExtensions
{
    public static Version SafeParseVersion(string? input)
    {
        
        if (string.IsNullOrWhiteSpace(input))
            return new Version(0, 0);

        // Strip leading 'v' or 'V' and trim whitespace
        string cleaned = input.Trim().TrimStart('v', 'V');

        // Try parsing
        var clean =  Version.TryParse(cleaned, out var parsed)
            ? parsed
            : new Version(0, 0);

        return clean;
    }

    public static List<ReleaseListItem> GetLatestByShortName(this IEnumerable<ReleaseListItem> items)
    {
        if (items == null) return new List<ReleaseListItem>();

        return items
            .GroupBy(i => i.ShortName)
            .Select(g =>
                g.OrderByDescending(r => SafeParseVersion(r.Version))
                .First()
            )
            .ToList();
    }
    public static List<string> Versions(this List<ReleaseListItem> allItems, string shortName)
    {
        return allItems
            .Where(r => r.ShortName == shortName)
            .OrderByDescending(r => SafeParseVersion(r.Version))
            .Select(r => SafeParseVersion(r.Version).ToString()).ToList(); // 👈 Project to string
    }

    public static ReleaseListItem Latest(this List<ReleaseListItem> allItems, string shortName)
    {
        return allItems
            .Where(r => r.ShortName == shortName)
            .OrderByDescending(r => SafeParseVersion(r.Version))
            .FirstOrDefault() ?? new ReleaseListItem();
    }

    public static ReleaseListItem Selected(this List<ReleaseListItem> allItems, string shortName , string version)
    {
        return allItems
            .Where(r => r.ShortName == shortName && SafeParseVersion( r.Version ) == SafeParseVersion( version ))
            .OrderByDescending(r => SafeParseVersion(r.Version))
            .FirstOrDefault() ?? new ReleaseListItem();
    }
}

    public class ReleaseInfo
    {
        public string Repo { get; set; } = string.Empty;
        public string Tag { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ReadMeUrl { get; set; } = string.Empty;
        public string ReadMeDocUrl { get; set; } = string.Empty;
        public DateTime Published { get; set; } = DateTime.Now;
        public bool IsLatest { get; set; } = false;
        public List<ZipFile> ZipFiles { get; set; } = new List<ZipFile>();
    }

    public class ZipFile
    {
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }

    public class ReleaseService
    {
        IPluginRegistry _registry;
        public Dictionary<string, List<ReleaseInfo>> Releases { get; private set; }

        private ReleaseService( IPluginRegistry registry, Dictionary<string, List<ReleaseInfo>> releases)
        {
            Releases = releases;
            _registry = registry;
    }

        public static async Task<ReleaseService> CreateAsync( IPluginRegistry registry , string url)
        {
            var loader = new ReleaseLoader();
            var releases = await loader.LoadReleasesAsync(url);
            return new ReleaseService( registry , releases);
        }

        public void print()
        {
            foreach (var repo in Releases.Keys)
            {
                Debug.WriteLine($"Repo: {repo}");
                foreach (var release in Releases[repo])
                {
                    Debug.WriteLine($"  - {release.Tag} ({(release.IsLatest ? "Latest" : "Old")})");
                    foreach (var zip in release.ZipFiles)
                    {
                        Debug.WriteLine($"    • {zip.Name} → {zip.Url}");
                    }
                }
            }
        }

        public IEnumerable<ReleaseListItem> GetReleaseItems( )
        {
            foreach (var repo in Releases.Keys)
            {
                var version = string.Empty;
                var ShortName = repo.Split("/").LastOrDefault() ?? string.Empty;
                IPlugin? plugin = _registry.Get(ShortName);
                if ( plugin != null )
                {
                    var temp = plugin.Version ?? string.Empty;
                    version = string.Join(".", temp.Split('.').Take(3));
            }
                    
                foreach (var release in Releases[repo])
                {
                    foreach (var zip in release.ZipFiles)
                    {
                        yield return new ReleaseListItem
                        {
                            Repo = repo,
                            Version = release.Tag,
                            IsLatest = release.IsLatest,
                            ZipName = zip.Name,
                            DownloadUrl = zip.Url,
                            ReadMeUrl = release.ReadMeUrl,
                            ReadMeDocUrl = release.ReadMeDocUrl,
                            Installed =  version,
                            ShortName = ShortName,
                        };
                    }
                }
            }
        }
    }

    public class ReleaseLoader
    {
        public async Task<Dictionary<string, List<ReleaseInfo>>> LoadReleasesAsync(string JsonUrl)
        {
            using var httpClient = new HttpClient();
            var json = await httpClient.GetStringAsync(JsonUrl);

            var releases = JsonSerializer.Deserialize<List<ReleaseInfo>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (releases is null) return new Dictionary<string, List<ReleaseInfo>>(); // Return an empty dictionary instead of null

            var dict = new Dictionary<string, List<ReleaseInfo>>();

            foreach (var release in releases)
            {
                if (!dict.ContainsKey(release.Repo))
                    dict[release.Repo] = new List<ReleaseInfo>();

                dict[release.Repo].Add(release);
            }

            return dict;
        }
    }


