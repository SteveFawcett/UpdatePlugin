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
        private static UpdateForm? _formInstance;
        public List<ToolStripItem>? ContextMenuItems { get; set; } = null;

        public UpdatePlugin(IConfiguration configuration, ILogger<IPlugin> logger , IPluginRegistry registry ) :
            base(configuration, CreateForm( logger , registry , configuration  ), Resources.icon, Stanza)
        {
            _logger = logger;

            ContextMenuItems = new List<ToolStripItem>()
            {
                new ToolStripMenuItem("Open", null, OnOpenClicked),
                new ToolStripSeparator(),
                new ToolStripSeparator(),
                new ToolStripSeparator(),
                new ToolStripSeparator(),
            };
        }

        private static UpdateForm CreateForm(ILogger<IPlugin> logger, IPluginRegistry registry, IConfiguration config)
        {
            _formInstance = new UpdateForm(logger, registry, config);
            return _formInstance;
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
            if(_formInstance != null)
                ShowScreen?.Invoke(this, _formInstance  );
        }
    }
}