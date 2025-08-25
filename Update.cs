using BroadcastPluginSDK.abstracts;
using BroadcastPluginSDK.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using UpdatePlugin.Properties;
using UpdatePlugin.Forms;

namespace UpdatePlugin
{
    public class UpdatePlugin : BroadcastPluginBase , IManager
    {

        private const string Stanza = "YOUR CONFIG STANZA";
        private ILogger<IPlugin> _logger;

        public UpdatePlugin(IConfiguration configuration, ILogger<IPlugin> logger , IPluginRegistry registry ) :
            base(configuration, new UpdateForm( logger , registry , configuration  ), Resources.icon, Stanza)
        {
            _logger = logger;
        }

        public event EventHandler<bool>? TriggerRestart;

        public void RequestRestart(bool isForced)
        {
            _logger.LogDebug("Requesting application restart. Forced: {IsForced}", isForced);
            TriggerRestart?.Invoke(this, isForced);
        }
    }
}