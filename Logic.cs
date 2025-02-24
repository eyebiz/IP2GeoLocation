﻿using Newtonsoft.Json;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace IP2GeoLocation
{
    class Logic
    {
        static byte[] entropy = Encoding.Unicode.GetBytes("SaltMeUpForThisSoundsGood");

        public static string EncryptString(SecureString input)
        {
            byte[] encryptedData = ProtectedData.Protect(
                Encoding.Unicode.GetBytes(ToInsecureString(input)),
                entropy,
                DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(encryptedData);
        }

        public static SecureString DecryptString(string encryptedData)
        {
            try
            {
                byte[] decryptedData = ProtectedData.Unprotect(
                    Convert.FromBase64String(encryptedData),
                    entropy,
                    DataProtectionScope.CurrentUser);
                return ToSecureString(Encoding.Unicode.GetString(decryptedData));
            }
            catch
            {
                return new SecureString();
            }
        }

        public static SecureString ToSecureString(string input)
        {
            SecureString secure = new SecureString();
            foreach (char c in input)
            {
                secure.AppendChar(c);
            }
            secure.MakeReadOnly();
            return secure;
        }

        public static string ToInsecureString(SecureString input)
        {
            string returnValue = string.Empty;
            IntPtr ptr = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(input);
            try
            {
                returnValue = System.Runtime.InteropServices.Marshal.PtrToStringBSTR(ptr);
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ZeroFreeBSTR(ptr);
            }
            return returnValue;
        }

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
            sb.AppendLine("    <add key=\"Port\" value=\"42022\" />");
            sb.AppendLine("    <add key=\"User\" value=\"root\" />");
            sb.AppendLine("    <add key=\"Pass\" value=\"secret-pass\" />");
            sb.AppendLine("  </appSettings>");
            sb.AppendLine("</configuration>");
            File.WriteAllText(String.Concat(Application.ExecutablePath, ".config"), sb.ToString());
        }

        public bool ValidateIPv4(string ipString)
        {
            if (ipString.Count(c => c == '.') != 3) return false;
            if (ipString.StartsWith("192.168.") || ipString.StartsWith("10.")) return false; // Remove LAN addresses
            return IPAddress.TryParse(ipString, out IPAddress address);
        }

        public string PingIP(string ip)
        {
            Ping p = new Ping();
            PingReply r;
            r = p.Send(ip);

            if (r.Status == IPStatus.Success)
            {
                 return "Ping to " + ip + " successful."
                   + " Response time: " + r.RoundtripTime.ToString() + " ms";
            }
            return "No response from " +ip;
        }

        public List<string> GetInfoFromSSHServer(SshClient client, string game)
        {
            SshCommand output;
            switch (game)
            {
                case "EA NHL":
                    output = client.RunCommand("cat /proc/net/nf_conntrack | grep sport=3659 | awk '{print $7}' | sed 's/dst=//g'");
                    break;
                case "Destiny 2":
                    output = client.RunCommand("cat /proc/net/nf_conntrack | grep sport=3097 | awk '{print $7}' | sed 's/dst=//g'");
                    break;
                case "Warframe":
                    output = client.RunCommand("cat /proc/net/nf_conntrack | grep 'sport=495.' | awk '{print $7}' | sed 's/dst=//g'");
                    break;
                case "PS4 Party":
                    output = client.RunCommand("cat /proc/net/nf_conntrack | grep src=10.0.0.104 | grep -v sport=3478 | grep -v sport=443 | grep -v dst=10.0.0.1 | awk '{print $7}' | sed 's/dst=//g'");
                    break;
                case "Note5":
                    output = client.RunCommand("cat /proc/net/nf_conntrack | grep sport=8999 | awk '{print $7}' | sed 's/dst=//g'");
                    break;
                case "HTTPS":
                    output = client.RunCommand("cat /proc/net/nf_conntrack | grep dport=443 | awk '{print $8}' | sed 's/dst=//g'");
                    break;
                default:
                    output = client.RunCommand("cat /proc/net/nf_conntrack | grep sport=3659 | awk '{print $7}' | sed 's/dst=//g'");
                    break;
            }
            //var splitString = output.Result.Split(new[] { "\n" }, StringSplitOptions.None);
            var unsortedIPs = output.Result.Split('\n').ToList();
            //var unsortedIPs = output.Result.Split(new[] { "\n" }, StringSplitOptions.None).ToList();
            unsortedIPs.RemoveAll(elem => !ValidateIPv4(elem));
            unsortedIPs = unsortedIPs.Distinct().ToList();
            var sortedIPs = unsortedIPs
                .Select(Version.Parse)
                .OrderBy(arg => arg)
                .Select(arg => arg.ToString())
                .ToList();
            return sortedIPs;
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
