using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

namespace IP2GeoLocation
{
    partial class AboutBox1 : Form
    {
        public AboutBox1()
        {
            InitializeComponent();
            this.Text = String.Format("About {0}", AssemblyTitle);
            this.labelProductName.Text = AssemblyProduct;
            this.labelVersion.Text = String.Format("Version {0}", AssemblyVersion);
            this.labelCopyright.Text = AssemblyCopyright;
            this.labelDate.Text = Assembly.GetExecutingAssembly().GetLinkerTime().ToString();
            //this.labelCompanyName.Text = AssemblyCompany;
            //this.textBoxDescription.Text = AssemblyDescription;
            FileVersionInfo entityFrameworkDLL = FileVersionInfo.GetVersionInfo(Application.StartupPath + @"\EntityFramework.dll");
            FileVersionInfo gMapNETCoreDLL = FileVersionInfo.GetVersionInfo(Application.StartupPath + @"\GMap.NET.Core.dll");
            FileVersionInfo newtonsoftJsonDLL = FileVersionInfo.GetVersionInfo(Application.StartupPath + @"\Newtonsoft.Json.dll");
            FileVersionInfo renciSshNetDLL = FileVersionInfo.GetVersionInfo(Application.StartupPath + @"\Renci.SshNet.dll");
            this.textBoxDescription.Clear();
            this.textBoxDescription.AppendText(entityFrameworkDLL.InternalName + " v" + entityFrameworkDLL.FileVersion + Environment.NewLine);
            this.textBoxDescription.AppendText(gMapNETCoreDLL.InternalName + " v" + gMapNETCoreDLL.FileVersion + Environment.NewLine);
            this.textBoxDescription.AppendText(newtonsoftJsonDLL.InternalName + " v" + newtonsoftJsonDLL.FileVersion + Environment.NewLine);
            this.textBoxDescription.AppendText(renciSshNetDLL.InternalName + " v" + renciSshNetDLL.FileVersion + Environment.NewLine);
        }

        #region Assembly Attribute Accessors

        public string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }
        #endregion
    }
}
