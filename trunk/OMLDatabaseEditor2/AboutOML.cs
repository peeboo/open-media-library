using System;
using System.Reflection;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Text;

namespace OMLDatabaseEditor
{
    partial class AboutOML : Form
    {
            Assembly _assembly;
            Stream _txtStream;
            StreamReader _txtStreamReader;

        public AboutOML()
        {
            InitializeComponent();
            this.Text = String.Format("About {0}", AssemblyTitle);
            this.labelProductName.Text = AssemblyProduct;
            //this.labelVersion.Text = String.Format("Version {0}", AssemblyVersion);
            this.labelCopyright.Text = AssemblyCopyright;
            //this.labelCompanyName.Text = AssemblyCompany;
            //this.textBoxDescription.Text = AssemblyDescription;

            textBoxDescription.Text = BuildCreditsString();

            try
            {
                _assembly = Assembly.GetExecutingAssembly();
                _txtStream = _assembly.GetManifestResourceStream("OMLDatabaseEditor.Revision.txt");
                _txtStreamReader = new StreamReader(_txtStream);
                labelVersion.Text += _txtStreamReader.ReadToEnd();
                _txtStreamReader.Close();
                _txtStream.Close();
            }
            catch (Exception)
            { }
        }

        private string BuildCreditsString()
        {
            StringBuilder CreditsText = new StringBuilder();
            
            Stream creditsStream = null;
            Assembly a = Assembly.GetExecutingAssembly();

            creditsStream = a.GetManifestResourceStream("OMLDatabaseEditor.Resources.Credits.xml");

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(creditsStream);
            XPathNavigator nav = xDoc.CreateNavigator();
            XPathNodeIterator it = nav.Select("Credits/Developers/Person");

            CreditsText.AppendLine("DEVELOPMENT TEAM:");
            while (it.MoveNext())
            {
                CreditsText.AppendLine(it.Current.Value);
            }
            CreditsText.AppendLine("");
            it = nav.Select("Credits/Contributors/Person");

            CreditsText.AppendLine("COMPANIES AND INDIVIDUALS:");
            while (it.MoveNext())
            {
                CreditsText.AppendLine(it.Current.Value);
            }
            CreditsText.AppendLine("");
            it = nav.Select("Credits/Special/Person");

            CreditsText.AppendLine("SPECIAL THANKS:");
            while (it.MoveNext())
            {
                CreditsText.AppendLine(it.Current.Value);
            }

            CreditsText.AppendLine("");
            CreditsText.AppendLine("And many others that we have missed, sorry we missed you but thanks for your support.");
            
            return CreditsText.ToString();
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

        private void okButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }


    }
}
