using System;
using System.Windows.Forms;

namespace IP2GeoLocation
{
    public partial class Form1 : Form
    {
        Logic l = new Logic();

        public Form1()
        {
            InitializeComponent();
        }

        private void BtnParse_Click(object sender, EventArgs e)
        {
            string ip = lbIP.Items[0].ToString();
            lbDNS.Items.Clear();
            lbDNS.Items.Add(ip);
            lbDNS.Items.Add(ip);
            lbDNS.Items.Add(ip);

        }

        private void BtnGetIPs_Click(object sender, EventArgs e)
        {
            lbIP.Items.Clear();
            lbIP.Items.Add("62.72.236.188");
            lbIP.Items.Add("90.230.121.215");
            lbIP.Items.Add("155.4.132.224");
            lbIP.Items.Add("86.0.129.209");
            lbIP.Items.Add("93.227.136.238");
        }

        private void lbDNS_SelectedValueChanged(object sender, EventArgs e)
        {
            ListBox lb = (ListBox)sender;
            string country = l.GetUserCountryByIp((string)lb.SelectedItem);
            lbGeo.Items.Clear();
            lbGeo.Items.Add(country);
        }
    }
}
