using Renci.SshNet;
using System;
using System.Collections.Generic;
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
        private string _selectedMenuItem;
        private Timer timer1;
        private int timerCounter;

        public Form1()
        {
            InitializeComponent();
            if (!File.Exists(String.Concat(Application.ExecutablePath, ".config")))
            {
                l.CreateAppConfig();
            }
            comboBoxGames.Items.Clear();
            comboBoxGames.Items.AddRange(new string[] { "EA NHL", "Destiny 2", "PS4 Party", "Note5", "HTTPS" });
            comboBoxGames.SelectedIndex = 0;
            ButtonGetIPs.Enabled = false;

            this.Activated += new EventHandler(Form1_Activated);
            this.ListBoxIP.SelectedValueChanged += new EventHandler(ListBoxIP_SelectedValueChanged);
            this.Closed += new EventHandler(Form1_Closed);

            var tsMenuCopy = new ToolStripMenuItem { Text = "Copy to clipboard" };
            tsMenuCopy.Click += tsMenuCopy_Click;
            var tsMenuCopyIP = new ToolStripMenuItem { Text = "Copy to clipboard" };
            tsMenuCopyIP.Click += tsMenuCopy_Click;
            var tsMenuPingIP = new ToolStripMenuItem { Text = "Ping IP" };
            tsMenuPingIP.Click += tsMenuPingIP_Click;
            ListBoxIPMenu.Items.AddRange(new ToolStripItem[] { tsMenuCopyIP, tsMenuPingIP });
            ListBoxMenu.Items.AddRange(new ToolStripItem[] { tsMenuCopy });
            ListBoxIP.MouseDown += new MouseEventHandler(ListBox_MouseDown);
            ListBoxGeo.MouseDown += new MouseEventHandler(ListBox_MouseDown);
            ListBoxPing.MouseDown += new MouseEventHandler(ListBox_MouseDown);
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            if (File.Exists(String.Concat(Application.ExecutablePath, ".config")))
            {
                config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                sshServer = config.AppSettings.Settings["SSHserver"].Value;
                sshPort = Int32.Parse(config.AppSettings.Settings["Port"].Value);
                sshUser = config.AppSettings.Settings["User"].Value;
                sshPass = Logic.ToInsecureString(Logic.DecryptString(config.AppSettings.Settings["Pass"].Value));
            }
        }

        private void ListBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ListBox lb = (ListBox)sender;
                var index = lb.IndexFromPoint(e.Location);
                if (index != ListBox.NoMatches)
                {
                    lb.SelectedIndex = index;
                    _selectedMenuItem = lb.Items[index].ToString();
                    // Show different context menus for each ListBox
                    switch (lb.Name)
                    {
                        case "ListBoxIP":
                            ListBoxIPMenu.Show(Cursor.Position);
                            ListBoxIPMenu.Visible = true;
                            break;
                        default:
                            ListBoxMenu.Show(Cursor.Position);
                            ListBoxMenu.Visible = true;
                            break;
                    }
                }
                else
                {
                    ListBoxMenu.Visible = false;
                }
            }
        }

        private void tsMenuCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(_selectedMenuItem,false,5,200);
        }

        private void tsMenuPingIP_Click(object sender, EventArgs e)
        {
            ListBoxPing.Items.Clear();
            ListBoxPing.Items.Add(l.PingIP(_selectedMenuItem));
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
                ButtonGetIPs.Enabled = false;
            }
            else
            {
                ListBoxIP.DataSource = null;
                string selectedGame = comboBoxGames.GetItemText(comboBoxGames.SelectedItem);
                List<string> list = l.GetInfoFromSSHServer(client, selectedGame);
                ListBoxIP.DataSource = list;
                timerCounter = 600;
            }
        }

        private void configToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigForm aboutBox = new ConfigForm();
            aboutBox.ShowDialog();
        }

        private void ButtonConnect_Click(object sender, EventArgs e)
        {
            tsStatus.Text = "Connecting to " + sshServer;
            client = new SshClient(sshServer, sshPort, sshUser, sshPass);
            client.ConnectionInfo.Timeout = TimeSpan.FromMinutes(10);
            try
            {
                client.Connect();
                if (client.IsConnected)
                {
                    tsStatus.Text = "Connected to " + sshServer;
                    ButtonGetIPs.Enabled = true;
                    timerCounter = 600;
                    timer1 = new Timer();
                    timer1.Tick += new EventHandler(timer1_Tick);
                    timer1.Interval = 1000; // 1 second
                    timer1.Start();
                    lblTimer.Text = timerCounter.ToString();
                }
            }
            catch (Exception ex)
            {
                tsStatus.Text = "Connection failed: " + ex.Message;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timerCounter--;
            if (timerCounter == 0)
                timer1.Stop();
            lblTimer.Text = timerCounter.ToString();
        }

        private void ListBoxIP_SelectedValueChanged(object sender, EventArgs e)
        {
            ListBox lb = (ListBox)sender;
            string selectedItem = (string)lb.SelectedItem;
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
    }
}

