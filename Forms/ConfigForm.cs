using BroadcastPluginSDK.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using UpdatePlugin.Properties;

namespace UpdatePlugin.Forms
{
    public partial class ConfigForm : UserControl
    {
        IPluginRegistry? _pluginRegistry;
        ILogger<IPlugin>? _logger;
        IConfiguration? _configuration;
        ToolTip toolTip = new ToolTip();

        public ConfigForm() => InitializeComponent();

        public ConfigForm(IConfiguration config, IPluginRegistry registry, ILogger<IPlugin> logger)
        {
            InitializeComponent();

            _logger = logger;
            _pluginRegistry = registry;
            _configuration = config;

            txtRepository.Text = _configuration["RepositoryUrl"] ?? string.Empty;
            txtInstallLocation.Text = _configuration["PluginInstallPath"] ?? string.Empty;

            setSaveButton();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (_configuration == null) return;

            if (_pluginRegistry?.Get("UpdatePlugin") is UpdatePlugin manager)
            {
                _logger?.LogDebug("Plugin Request for configuration dump via IManager.");
                _configuration["RepositoryUrl"] = txtRepository.Text;
                _configuration["PluginInstallPath"] = txtInstallLocation.Text;
                manager.ConfigurationDump();
            }

            setSaveButton();
        }

        private void btnRevertInstall_Click(object sender, EventArgs e) => setText(txtInstallLocation, "PluginInstallPath");

        private void btnRevertRepository_Click(object sender, EventArgs e) => setText(txtRepository, "RepositoryUrl");

        private void txtRepository_TextChanged(object sender, EventArgs e) => setSaveButton();

        private void txtInstallLocation_TextChanged(object sender, EventArgs e) => setSaveButton();

        private void setText(TextBox box, string key)
        {
            if (_configuration == null) return;
            box.Text = _configuration[key] ?? string.Empty;
        }

        private void setSaveButton()
        {
            if (_configuration == null)
            {
                btnRevertInstall.SetOpacity(Resources.undo, 0.4f);
                btnRevertRepository.SetOpacity(Resources.undo, 0.4f);
                toolTip.SetToolTip(btnRevertInstall, "No changes to revert");
                toolTip.SetToolTip(btnRevertRepository, "No changes to revert");
                return;
            }

            SetRevertButtonState(btnRevertInstall, txtInstallLocation.Text, _configuration["PluginInstallPath"], "install path");
            SetRevertButtonState(btnRevertRepository, txtRepository.Text, _configuration["RepositoryUrl"], "repository URL");

            btnSave.Enabled = btnRevertInstall.Enabled || btnRevertRepository.Enabled;

            if(btnSave.Enabled)
            {
                toolTip.SetToolTip(btnSave, "Save changes to config");
                btnSave.BackColor = Color.LightGreen;
            }
            else
            {
                toolTip.SetToolTip(btnSave, "No changes to save — matches config");
                btnSave.BackColor = SystemColors.Control;
            }
        }

        private void SetRevertButtonState(Button button, string currentValue, string? configValue, string label)
        {
            bool changed = currentValue != configValue;
            button.Enabled = changed;
            button.SetOpacity(Resources.undo, changed ? 1f : 0.4f);

            string tooltipText = changed
                ? $"Revert {label} to saved value"
                : $"No changes to revert — matches config";

            toolTip.SetToolTip(button, tooltipText);
        }
    }

    public static class ButtonExtensions
    {
        private static readonly Dictionary<(Image, float), Image> _imageCache = new();

        public static void SetOpacity(this Button button, Image image, float opacity)
        {
            if (image == null) return;

            var key = (image, opacity);
            if (!_imageCache.TryGetValue(key, out var faded))
            {
                faded = new Bitmap(image.Width, image.Height);
                using (Graphics g = Graphics.FromImage(faded))
                {
                    ColorMatrix matrix = new ColorMatrix { Matrix33 = Math.Clamp(opacity, 0f, 1f) };
                    ImageAttributes attributes = new ImageAttributes();
                    attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                    g.DrawImage(image, new Rectangle(0, 0, faded.Width, faded.Height),
                        0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
                }
                _imageCache[key] = faded;
            }

            button.BackgroundImage = faded;
            button.BackgroundImageLayout = ImageLayout.Stretch;
        }
    }
}
