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
            sb.AppendLine("    <add key=\"User\" value=\"root\" />");
            sb.AppendLine("    <add key=\"Pass\" value=\"secret-pass\" />");
            sb.AppendLine("  </appSettings>");
            sb.AppendLine("</configuration>");
            File.WriteAllText(String.Concat(Application.ExecutablePath, ".config"), sb.ToString());
        }

        public static bool ValidateIPv4(string ipString)
        {
            if (ipString.Count(c => c == '.') != 3) return false;
            IPAddress address;
            return IPAddress.TryParse(ipString, out address);
        }

        public void GetIPsFromSSHServer(string server, string user, string pass)
        {
            //Set up the SSH connection
            using (var client = new SshClient(server, user, pass))
            {
                //Start the connection
                client.Connect();
                var output = client.RunCommand("echo test");
                client.Disconnect();
                Console.WriteLine(output.Result);
            }
        }

        public string GetUserCountryByIp(string ip)
        {
            IpInfo ipInfo = new IpInfo();
            try
            {
                string info = new WebClient().DownloadString("http://ipinfo.io/" + ip);
                ipInfo = JsonConvert.DeserializeObject<IpInfo>(info);
                RegionInfo myRI1 = new RegionInfo(ipInfo.Country);
                ipInfo.Country = myRI1.EnglishName;
            }
            catch (Exception)
            {
                ipInfo.Country = null;
            }

            return ipInfo.Country;
        }

        public IpInfo GetIpInfo(string ip)
        {
            IpInfo ipInfo = new IpInfo();
            try
            {
                WebClient webClient = new WebClient() { Encoding = Encoding.UTF8 };
                string info = webClient.DownloadString("http://ipinfo.io/" + ip);
                ipInfo = JsonConvert.DeserializeObject<IpInfo>(info);
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
