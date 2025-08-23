using BroadcastPluginSDK.abstracts;
using BroadcastPluginSDK.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using UpdatePlugin.Properties;

namespace UpdatePlugin
{
    public class UpdatePlugin : BroadcastPluginBase
    {

        private const string Stanza = "YOUR CONFIG STANZA";
        private ILogger<IPlugin> _logger;

        public UpdatePlugin(IConfiguration configuration, ILogger<IPlugin> logger) :
            base(configuration, null, Resources.green, Stanza)
        {
            _logger = logger;
        }
    }
}