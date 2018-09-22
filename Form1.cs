using Renci.SshNet;
using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace IP2GeoLocation
{
    public partial class Form1 : Form
    {
        Logic l = new Logic();
        Logic.IpInfo ipInfo;
        Configuration config;
        SshClient client;
        string sshServer, sshUser, sshPass;
        int sshPort;

        public Form1()
        {
            InitializeComponent();
            if (!File.Exists(String.Concat(Application.ExecutablePath, ".config")))
            {
                l.CreateAppConfig();
            }
            comboBoxGames.Items.Clear();
            comboBoxGames.Items.AddRange(new string[] { "NHL 19", "Destiny 2", "PS4 Party", "Note5" });
            comboBoxGames.SelectedIndex = 0;
            ButtonGetIPs.Enabled = false;

            this.Activated += new EventHandler(Form1_Activated);
            this.ListBoxIP.SelectedValueChanged += new EventHandler(ListBoxIP_SelectedValueChanged);
            this.Closed += new EventHandler(Form1_Closed);
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            if (File.Exists(String.Concat(Application.ExecutablePath, ".config")))
            {
                config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                sshServer = config.AppSettings.Settings["SSHserver"].Value;
                sshPort = Int32.Parse(config.AppSettings.Settings["Port"].Value);
                sshUser = config.AppSettings.Settings["User"].Value;
                sshPass = config.AppSettings.Settings["Pass"].Value;
            }
        }

        private void Form1_Closed(object sender, EventArgs e)
        {
            try
            {
                client.Disconnect();
                client.Dispose();
            }
            catch { }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 aboutBox = new AboutBox1();
            aboutBox.ShowDialog();
        }

        private void ButtonGetIPs_Click(object sender, EventArgs e)
        {
            if (!client.IsConnected)
            {
                tsStatus.Text = "Not connected to " + sshServer;
            }
            else
            {
                ListBoxIP.Items.Clear();
                string selectedGame = comboBoxGames.GetItemText(comboBoxGames.SelectedItem);
                string[] lines = l.GetInfoFromSSHServer(client, selectedGame);
                foreach (string line in lines)
                {
                    if ((line.IndexOf("192.168.", StringComparison.OrdinalIgnoreCase) >= 0) == false)
                    {
                        ListBoxIP.Items.Add(line);
                    }
                }
            }

            /*
            ListBoxIP.Items.Add("62.72.236.188");
            ListBoxIP.Items.Add("90.230.121.215");
            ListBoxIP.Items.Add("155.4.132.224");
            ListBoxIP.Items.Add("86.0.129.209");
            ListBoxIP.Items.Add("93.227.136.238");
            ListBoxIP.Items.Add("213.113.23.168");
            ListBoxIP.Items.Add("47.74.171.176");
            ListBoxIP.Items.Add("159.153.76");
            */

        }

        private void ButtonConnect_Click(object sender, EventArgs e)
        {
            tsStatus.Text = "Connecting to " + sshServer;
            client = new SshClient(sshServer, sshPort, sshUser, sshPass);
            client.ConnectionInfo.Timeout = TimeSpan.FromMinutes(10);
            client.Connect();
            //client = l.ConnectToSSHServer(sshServer, sshPort, sshUser, sshPass);
            if (client.IsConnected)
            {
                tsStatus.Text = "Connected to " + sshServer;
                ButtonGetIPs.Enabled = true;
            }
        }

        private void ListBoxIP_SelectedValueChanged(object sender, EventArgs e)
        {
            ListBox lb = (ListBox)sender;
            string selectedItem = (string)lb.SelectedItem;
            if (l.ValidateIPv4(selectedItem)) {
                ipInfo = l.GetIpInfo(selectedItem);

                ListBoxGeo.Items.Clear();
                ListBoxGeo.Items.Add("IP................: " + ipInfo.IP);
                ListBoxGeo.Items.Add("Hostname...: " + ipInfo.Hostname);
                ListBoxGeo.Items.Add("City.............: " + ipInfo.City);
                ListBoxGeo.Items.Add("Region........: " + ipInfo.Region);
                ListBoxGeo.Items.Add("Country.......: " + ipInfo.Country);
                ListBoxGeo.Items.Add("Location......: " + ipInfo.Loc);
                ListBoxGeo.Items.Add("Organization: " + ipInfo.Org);
                ListBoxGeo.Items.Add("Postal code.: " + ipInfo.Postal);

                // Get latitude and longitude from ipInfo and display on GMap
                double lat = Double.Parse(ipInfo.Loc.Split(',')[0], CultureInfo.InvariantCulture);
                double lng = Double.Parse(ipInfo.Loc.Split(',')[1], CultureInfo.InvariantCulture);
                gMap.MapProvider = GMap.NET.MapProviders.GoogleMapProvider.Instance;
                GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerOnly;
                gMap.Position = new GMap.NET.PointLatLng(lat, lng);
                gMap.MinZoom = 5;
                gMap.MaxZoom = 100;
                gMap.Zoom = 10;
            }
            else
            {
                ListBoxGeo.Items.Clear();
                ListBoxGeo.Items.Add("Invalid IP");
            }
        }
    }
}

