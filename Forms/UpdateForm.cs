using Broadcast.Classes;
using BroadcastPluginSDK.Classes;
using BroadcastPluginSDK.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using UpdatePlugin.Classes;
using UpdatePlugin.Properties;

namespace UpdatePlugin.Forms;

public partial class UpdateForm : UserControl, IInfoPage
{
    private readonly ILogger<IPlugin> _logger;
    private readonly PluginCardRenderer _renderer = new();
    private readonly IConfiguration _configuration;
    private readonly IPluginRegistry _registry;
    private readonly PluginUpdater _updates;
    private ReleaseListItem[] _releases = Array.Empty<ReleaseListItem>();
    public bool Locked { get; set; } = false;

    private ReleaseListItem? selected = null;

    public UpdateForm(ILogger<IPlugin> logger, IPluginRegistry registry, IConfiguration configuration  )
    {
        _logger = logger;
        _registry = registry;
        _configuration = configuration;
        _updates = new PluginUpdater(_configuration, _registry, _logger);

        _logger.LogInformation("Starting Update Form");

        InitializeComponent();
        UpdateButton(null);
        ButtonStatus();

        pictureBox1.Image = Resources.icon;
    }

    private void DisplayLink(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            _logger.LogWarning("No URL provided to display link.");
            return;
        }
        LinkLabel.Link link = new();
        link.LinkData = url;
        link.Start = 0;
        link.Length = linkLabel1.Text.Length;

        linkLabel1.Links.Clear();
        linkLabel1.Tag = url;
        linkLabel1.Links.Add(link);
    }

    private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        if (e.Link?.LinkData is not string url || string.IsNullOrWhiteSpace(url))
        {
            _logger.LogWarning("Link data is null or not a valid URL string.");
            return;
        }

        if (Uri.TryCreate(url, UriKind.Absolute, out var uriResult) &&
            (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
        {
            _logger.LogInformation("Opening link: {Link}", url);
            try
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to open link: {Link}", url);
            }
        }
        else
        {
            _logger.LogWarning("Link data is not a valid absolute HTTP/HTTPS URL.");
        }
    }

    private void UpdateCombo(ReleaseListItem release)
    {

        if (release == null)
        {
            comboBox1.Items.Clear();
            comboBox1.Text = "No version available";
            comboBox1.Enabled = false;
            _logger.LogWarning("ReleaseListItem is null, cannot update combo box.");
            return;
        }

        // Fix: Convert array to List before calling Versions extension method

        var versionList = _releases.ToList().Versions(release.ShortName);

        if (versionList == null || versionList.Count == 0)
        {
            comboBox1.Items.Clear();
            comboBox1.Text = "No version available";
            comboBox1.Enabled = false;
            _logger.LogWarning("No versions found for the short name: {ShortName}", release.ShortName);
            return;
        }

        comboBox1.Items.Clear();
        comboBox1.Items.AddRange(versionList.ToArray());
        comboBox1.Enabled = true;

        if (release.Installed != null && versionList.Contains(release.Installed))
        {
            comboBox1.Text = release.Installed;
        }
        else
        {
            comboBox1.Text = versionList[0]; // Default to the first version if installed version is not found
        }
    }

    private void UpdateButton(ReleaseListItem? release)
    {
        if (release == null)
        {
            actionBtn.Visible = false;
            actionBtn.Enabled = false;
            return;
        }
        if (string.Equals(release.Installed, release.Version, StringComparison.Ordinal))
        {
            actionBtn.Visible = false;
            actionBtn.Enabled = false;
            return;
        }
        else if (string.IsNullOrEmpty(release.Installed))
        {
            actionBtn.Text = "Install";
            actionBtn.Enabled = true;
        }
        else
        {
            actionBtn.Text = "Update";
            actionBtn.Enabled = true;
        }
        actionBtn.Visible = true;
        actionBtn.Enabled = true;
    }

    private void ListBox1_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (listBox1.SelectedItem is ReleaseListItem selected)
        {
            this.selected = selected;
            if (!string.IsNullOrWhiteSpace(selected.ReadMe))
            {
                richTextBox1.Rtf = MarkdownToRtfConverter.Convert(selected.ReadMe);
                DisplayLink(selected.ReadMeDocUrl);
                UpdateCombo(selected);
                UpdateButton(selected);
            }
            else
            {
                richTextBox1.Rtf = MarkdownToRtfConverter.Convert("# README not found or failed to load");
                DisplayLink(selected.Repo);
                UpdateButton(null);
                comboBox1.Items.Clear();
                comboBox1.Text = "No README available";
                comboBox1.Enabled = false;
            }
        }
        else
        {
            _logger.LogWarning("Selected item is not a ReleaseListItem");
            richTextBox1.Rtf = MarkdownToRtfConverter.Convert("# README not found or failed to load");
        }
    }

    private async void UpdateForm_Load(object sender, EventArgs e)
    {
        await Task.Delay(100); // Optional: let UI settle
        RefreshListBoxAsync();
    }

    private void ListBox1_DrawItem(object? sender, DrawItemEventArgs e)
    {
        if (e.Index < 0 || e.Index >= listBox1.Items.Count) return;

        var item = (ReleaseListItem)listBox1.Items[e.Index];
        _renderer.Draw(e.Graphics, e.Bounds, item, (e.State & DrawItemState.Selected) != 0);

        e.DrawFocusRectangle();
    }

    private async void RefreshListBoxAsync()
    {
        _releases = await _updates.GetReleases();

        foreach (var release in _releases.GetLatestByShortName())
        {
            if (selected == null)
            {
                selected = release;
                richTextBox1.Rtf = MarkdownToRtfConverter.Convert(release.ReadMe);
                DisplayLink(release.ReadMeDocUrl);
                UpdateCombo(release);
                UpdateButton(release);
            }

            listBox1.Items.Add(release);
        }
    }

    private void ComboBox1_DrawItem(object sender, DrawItemEventArgs e)
    {
        if (e.Index < 0) return;

        // Get the item and its text
        var item = comboBox1.Items[e.Index];

        if (item is null) return;
        if (selected is null) return;

        string text = item.ToString() ?? "0.0.0"; // Or item.Version if it's a Release object

        // Determine if this is the installed version
        bool isInstalled = (item is string s) && string.Equals(s, selected.Installed, StringComparison.Ordinal);

        // Set colors
        Color backColor = Color.White;
        Color textColor;

        if (isInstalled)
        {
            // Always highlight installed version
            textColor = Color.Blue;
            backColor = Color.LightYellow; // Optional: subtle background hint
        }
        else
        {
            // Use selection state for others
            textColor = (e.State & DrawItemState.Selected) == DrawItemState.Selected
                ? Color.Blue
                : Color.Black;
        }

        // Fill background
        using (SolidBrush backgroundBrush = new(backColor))
            e.Graphics.FillRectangle(backgroundBrush, e.Bounds);

        // Use a fallback font if e.Font is null
        var font = e.Font ?? SystemFonts.DefaultFont;

        // Draw text
        using (SolidBrush textBrush = new(textColor))
            e.Graphics.DrawString(text, font, textBrush, e.Bounds);

        // Optional: draw focus rectangle
        e.DrawFocusRectangle();
    }


    public Control GetControl()
    {
        return this;
    }

    private void actionBtn_Click(object sender, EventArgs e)
    {

        if (sender is not Button btn || selected is null)
        {
            _logger.LogWarning("Action button clicked but sender is not a Button or no release selected.");
            return;
        }

        if (_releases == null || _releases.Length == 0)
        {
            _logger.LogWarning("No releases available to perform action.");
            return;
        }

        var SelectedVersion = _releases.ToList().Selected(selected.ShortName, comboBox1.Text) ?? selected;

        ButtonStatus(false);

        if (string.Equals(btn.Text, "Install", StringComparison.OrdinalIgnoreCase))
        {
            _ = Downloader.Install( this , _configuration, _logger, SelectedVersion);
            _registry.Restart = true;
        }
        else if (string.Equals(btn.Text, "Update", StringComparison.OrdinalIgnoreCase))
        {
            _ = Downloader.Install(this , _configuration, _logger, SelectedVersion);
            _registry.Restart = true;
        }
        else
        {
            _logger.LogWarning("Action button clicked with unrecognized text: {ButtonText}", btn.Text);
        }

        ButtonStatus();
    }

    private void ButtonStatus( bool active = true )
    {
        if (_registry.Restart && active )
        {
            Restart.Visible = true;
            Restart.Enabled = true;
            Restart.BackColor = SystemColors.Control;
            Restart.ForeColor = SystemColors.ControlText;
        }
        else
        {
            Restart.Visible = false;
            Restart.Enabled = false;
            Restart.BackColor = SystemColors.ControlDark;
            Restart.ForeColor = SystemColors.GrayText;
        }
    }
    private void Restart_Click(object sender, EventArgs e)
    {
        _logger.LogInformation("Restart requested by user.");

        if (_registry.Get("UpdatePlugin") is UpdatePlugin manager)
        {
            _logger.LogDebug("Plugin {Name} supports restart via IManager.", "UpdatePlugin");
            manager.RequestRestart(true);
        }
        else
        {
            _logger.LogWarning("Plugin 'UpdatePlugin' does not implement IManager. Restart skipped.");
        }
    }
}