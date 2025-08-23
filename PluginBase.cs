using BroadcastPluginSDK.abstracts;
using BroadcastPluginSDK.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using UpdatePlugin.Properties;
using UpdatePlugin.Forms;

namespace UpdatePlugin
{
    public class UpdatePlugin : BroadcastPluginBase
    {

        private const string Stanza = "YOUR CONFIG STANZA";
        private ILogger<IPlugin> _logger;

        public UpdatePlugin(IConfiguration configuration, ILogger<IPlugin> logger , IPluginRegistry registry ) :
            base(configuration, new UpdateForm( logger , registry , configuration  ), Resources.green, Stanza)
        {
            _logger = logger;
        }
    }
}