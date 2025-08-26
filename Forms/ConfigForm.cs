using Microsoft.Extensions.Configuration;
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
        public ConfigForm()
        {
            InitializeComponent();
        }

        public ConfigForm( IConfiguration config)
        {
            InitializeComponent();

            textBox1.Text = config["RepositoryUrl"] ?? string.Empty;
            textBox2.Text = config["PluginInstallPath"] ?? string.Empty;
        }
    }
}
