using System;
using System.Configuration;
using System.IO;
using System.Windows.Forms;

namespace IP2GeoLocation
{
    public partial class Form1 : Form
    {
        Logic l = new Logic();
        Logic.IpInfo ipInfo;
        Configuration config;
        string sshServer, sshUser, sshPass;

        public Form1()
        {
            InitializeComponent();
            if (!File.Exists(String.Concat(Application.ExecutablePath, ".config")))
            {
                l.CreateAppConfig();
            }
            this.Activated += new EventHandler(Form1_Activated);
            this.ListBoxIP.SelectedValueChanged += new EventHandler(this.ListBoxIP_SelectedValueChanged);


        }
        private void Form1_Activated(object sender, EventArgs e)
        {
            if (File.Exists(String.Concat(Application.ExecutablePath, ".config")))
            {
                config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                sshServer = config.AppSettings.Settings["SSHserver"].Value;
                sshUser = config.AppSettings.Settings["User"].Value;
                sshPass = config.AppSettings.Settings["Pass"].Value;
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 aboutBox = new AboutBox1();
            aboutBox.ShowDialog();
        }

        private void BtnGetIPs_Click(object sender, EventArgs e)
        {

            ListBoxIP.Items.Clear();
            ListBoxIP.Items.Add("62.72.236.188");
            ListBoxIP.Items.Add("90.230.121.215");
            ListBoxIP.Items.Add("155.4.132.224");
            ListBoxIP.Items.Add("86.0.129.209");
            ListBoxIP.Items.Add("93.227.136.238");
            ListBoxIP.Items.Add("213.113.23.168");
            ListBoxIP.Items.Add("47.74.171.176");
            ListBoxIP.Items.Add("159.153.76.50");

        }

        private void BtnParse_Click(object sender, EventArgs e)
        {
            //string ip = lbIP.Items[0].ToString();
            //lbDNS.Items.Clear();
            //lbDNS.Items.Add(ip);
            //lbDNS.Items.Add(ip);
            //lbDNS.Items.Add(ip);

        }

        private void ListBoxIP_SelectedValueChanged(object sender, EventArgs e)
        {
            ListBox lb = (ListBox)sender;
            ipInfo = l.GetIpInfo((string)lb.SelectedItem);
            
            ListBoxGeo.Items.Clear();
            ListBoxGeo.Items.Add("IP................: "+ipInfo.IP);
            ListBoxGeo.Items.Add("Hostname...: "+ipInfo.Hostname);
            ListBoxGeo.Items.Add("City.............: "+ipInfo.City);
            ListBoxGeo.Items.Add("Region........: "+ipInfo.Region);
            ListBoxGeo.Items.Add("Country.......: "+ipInfo.Country);
            ListBoxGeo.Items.Add("Location......: "+ipInfo.Loc);
            ListBoxGeo.Items.Add("Organization: "+ipInfo.Org);
            ListBoxGeo.Items.Add("Postal code.: "+ipInfo.Postal);
        }
    }
}

