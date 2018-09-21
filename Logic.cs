using Newtonsoft.Json;
using Renci.SshNet;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace IP2GeoLocation
{
    class Logic
    {
        public void CreateAppConfig()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            sb.AppendLine("<configuration>");
            sb.AppendLine("  <startup>");
            sb.AppendLine("    <supportedRuntime version=\"v4.0\" sku=\".NETFramework,Version=v4.6.1\" />");
            sb.AppendLine("  </startup>");
            sb.AppendLine("  <appSettings>");
            sb.AppendLine("    <add key=\"SSHserver\" value=\"10.0.0.1\" />");
            sb.AppendLine("    <add key=\"Port\" value=\"62022\" />");
            sb.AppendLine("    <add key=\"User\" value=\"root\" />");
            sb.AppendLine("    <add key=\"Pass\" value=\"secret-pass\" />");
            sb.AppendLine("  </appSettings>");
            sb.AppendLine("</configuration>");
            File.WriteAllText(String.Concat(Application.ExecutablePath, ".config"), sb.ToString());
        }

        public bool ValidateIPv4(string ipString)
        {
            if (ipString.Count(c => c == '.') != 3) return false;
            IPAddress address;
            return IPAddress.TryParse(ipString, out address);
        }

        public string[] GetInfoFromSSHServer(SshClient client, string game)
        {
            SshCommand output;
            switch (game)
            {
                case "NHL 19":
                    output = client.RunCommand("cat /proc/net/nf_conntrack | grep sport=3659 | awk '{print $7}' | sed 's/dst=//g'");
                    break;
                case "Destiny 2":
                    output = client.RunCommand("cat /proc/net/nf_conntrack | grep sport=3097 | awk '{print $7}' | sed 's/dst=//g'");
                    break;
                case "PS4 Party":
                    output = client.RunCommand("cat /proc/net/nf_conntrack | grep sport=9307 | awk '{print $7}' | sed 's/dst=//g'");
                    break;
                case "Note5":
                    output = client.RunCommand("cat /proc/net/nf_conntrack | grep sport=8999 | awk '{print $7}' | sed 's/dst=//g'");
                    break;
                default:
                    output = client.RunCommand("cat /proc/net/nf_conntrack | grep sport=3659 | awk '{print $7}' | sed 's/dst=//g'");
                    break;
            }
        
            var splitString = output.Result.Split(new[] { "\n" }, StringSplitOptions.None);
            return splitString;
        }

        public IpInfo GetIpInfo(string ip)
        {
            IpInfo ipInfo = new IpInfo();
            try
            {
                WebClient webClient = new WebClient() { Encoding = Encoding.UTF8 };
                string info = webClient.DownloadString("http://ipinfo.io/" + ip);
                ipInfo = JsonConvert.DeserializeObject<IpInfo>(info);
                
                // Insert full country name
                RegionInfo myRI1 = new RegionInfo(ipInfo.Country);
                ipInfo.Country = myRI1.EnglishName;
            }
            catch (Exception)
            {
            }
            return ipInfo;

        }

        public class IpInfo
        {

            [JsonProperty("ip")]
            public string IP { get; set; }

            [JsonProperty("hostname")]
            public string Hostname { get; set; } = "N/A";

            [JsonProperty("city")]
            public string City { get; set; } = "N/A";

            [JsonProperty("region")]
            public string Region { get; set; } = "N/A";

            [JsonProperty("country")]
            public string Country { get; set; } = "N/A";

            [JsonProperty("loc")]
            public string Loc { get; set; } = "N/A";

            [JsonProperty("org")]
            public string Org { get; set; } = "N/A";

            [JsonProperty("postal")]
            public string Postal { get; set; } = "N/A";
        }

    }
}
