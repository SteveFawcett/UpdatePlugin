using BroadcastPluginSDK.abstracts;
using BroadcastPluginSDK.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using UpdatePlugin.Properties;
using UpdatePlugin.Forms;
using BroadcastPluginSDK;

namespace UpdatePlugin
{
    public class UpdatePlugin : BroadcastPluginBase , IManager
    {

        private const string Stanza = "YOUR CONFIG STANZA";
        private ILogger<IPlugin> _logger;
        private static UpdateForm? _updateForm;
        private static ConfigForm? _configForm ;
        private IConfiguration _configuration;
        public List<ToolStripItem>? ContextMenuItems { get; set; } = null;

        public UpdatePlugin(IConfiguration configuration, ILogger<IPlugin> logger , IPluginRegistry registry ) :
            base(configuration, CreateForm( logger , registry , configuration  ), Resources.icon, Stanza)
        {
            _logger = logger;
            _configuration = configuration;

            ContextMenuItems = new List<ToolStripItem>()
            {
                new ToolStripMenuItem("Open", null, OnOpenClicked),
                new ToolStripMenuItem("Configuration", null, OnConfigurationClicked),
            };
        }

        private static UpdateForm CreateForm(ILogger<IPlugin> logger, IPluginRegistry registry, IConfiguration config)
        {
            _updateForm = new UpdateForm(logger, registry, config);
            return _updateForm;
        }

        public event EventHandler<bool>? TriggerRestart;
        public event EventHandler<UserControl>? ShowScreen;

        public void RequestRestart(bool isForced)
        {
            _logger.LogDebug("Requesting application restart. Forced: {IsForced}", isForced);
            TriggerRestart?.Invoke(this, isForced);
        }

        private void OnOpenClicked(object? sender, EventArgs e)
        {
            if(_updateForm != null)
                ShowScreen?.Invoke(this, _updateForm  );
        }

        private void OnConfigurationClicked(object? sender, EventArgs e)
        {
            _configForm = new( _configuration );
            if (_configForm != null)
                ShowScreen?.Invoke(this, _configForm);
        }
    }
}