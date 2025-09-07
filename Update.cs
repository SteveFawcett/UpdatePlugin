using BroadcastPluginSDK.abstracts;
using BroadcastPluginSDK.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using UpdatePlugin.Forms;
using UpdatePlugin.Properties;

namespace UpdatePlugin
{
    public class UpdatePlugin : BroadcastPluginBase , IManager
    {

        private ILogger<IPlugin>? _logger;
        private static UpdateForm? _updateForm;
        private static ConfigForm? _configForm ;
        private IConfiguration? _configuration;
        private IPluginRegistry? _registry;
        public List<ToolStripItem>? ContextMenuItems { get; set; } = null;
        public bool Locked { get; set;  } = false;

        public UpdatePlugin() : base() { }

        public UpdatePlugin(IConfiguration configuration, ILogger<IPlugin> logger , IPluginRegistry registry ) :
            base(configuration, CreateForm( logger , registry , configuration  ), Resources.icon, null )
        {
            _logger = logger;
            _configuration = configuration;
            _registry = registry;

            ContextMenuItems = new List<ToolStripItem>()
            {
                new ToolStripMenuItem("Open", null, OnOpenClicked),
                new ToolStripMenuItem("Configuration", null, OnConfigurationClicked),
            };
        }

        private static UpdateForm CreateForm(ILogger<IPlugin> logger, IPluginRegistry registry, IConfiguration config)
        {
            _updateForm = new UpdateForm(logger, registry, config );
            return _updateForm;
        }

        public event EventHandler<bool>? TriggerRestart;
        public event EventHandler<UserControl>? ShowScreen;
        public event EventHandler? WriteConfiguration;

        public void ConfigurationDump()
        {
            WriteConfiguration?.Invoke(this, EventArgs.Empty  );
        }
        public void RequestRestart(bool isForced)
        {
            _logger?.LogDebug("Requesting application restart. Forced: {IsForced}", isForced);
            TriggerRestart?.Invoke(this, isForced);
        }

        private void OnOpenClicked(object? sender, EventArgs e)
        {
            if(_updateForm != null)
                ShowScreen?.Invoke(this, _updateForm  );
        }

        private void OnConfigurationClicked(object? sender, EventArgs e)
        {
            if (_configForm == null)
                _configForm = new(_configuration, _registry, _logger);
            if (_configForm != null)
                ShowScreen?.Invoke(this, _configForm);
        }
    }
}