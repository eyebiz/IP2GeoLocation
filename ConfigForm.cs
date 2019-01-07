using System;
using System.Configuration;
using System.Windows.Forms;

namespace IP2GeoLocation
{
    public partial class ConfigForm : Form
    {
        Configuration config;

        public ConfigForm()
        {
            InitializeComponent();
            config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            tbServer.Text = config.AppSettings.Settings["SSHserver"].Value;
            tbPort.Text = config.AppSettings.Settings["Port"].Value;
            tbUsername.Text = config.AppSettings.Settings["User"].Value;
            tbPassword.Text = Logic.ToInsecureString(Logic.DecryptString(config.AppSettings.Settings["Pass"].Value));
        }

        private void btnConfigSave_Click(object sender, EventArgs e)
        {
            config.AppSettings.Settings["SSHserver"].Value = tbServer.Text;
            config.AppSettings.Settings["Port"].Value = tbPort.Text;
            config.AppSettings.Settings["User"].Value = tbUsername.Text;
            config.AppSettings.Settings["Pass"].Value = Logic.EncryptString(Logic.ToSecureString(tbPassword.Text));
            config.Save(ConfigurationSaveMode.Modified);
            this.Close();
        }

        private void btnConfigDiscard_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
