using BroadcastPluginSDK.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UpdatePlugin.Forms
{
    public partial class ConfigForm : UserControl
    {
        IPluginRegistry _pluginRegistry;
        ILogger<IPlugin> _logger;
        public ConfigForm()
        {
            InitializeComponent();
        }

        public ConfigForm(IConfiguration config , IPluginRegistry registry , ILogger<IPlugin> logger) 
        {
            InitializeComponent();

            _logger = logger;
            _pluginRegistry = registry;

            textBox1.Text = config["RepositoryUrl"] ?? string.Empty;
            textBox2.Text = config["PluginInstallPath"] ?? string.Empty;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (_pluginRegistry.Get("UpdatePlugin") is UpdatePlugin manager)
            {
                _logger.LogDebug("Plugin {Name} supports Request for configuration dump via IManager.", "ConfigurationDump");
                manager.ConfigurationDump();
            }
        }
    }
}
