using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using System.Xml.Linq;

namespace VPNBuddy
{
    public partial class MainForm : Form
    {
        private SecurityManager _securityManager;

        public MainForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string password = Prompt.ShowDialog("Enter password", "Password");
            _securityManager = new SecurityManager(password);
            LoadConfig();
        }

        private void LoadConfig()
        {
            try
            {
                Write(string.Empty, true);
                var configDoc = XDocument.Load("config.xml");
                var doc = configDoc.Descendants("vpn");
                GetVpnDetails(doc);
            }
            catch (Exception ex)
            {
                Write(ex);
            }
        }

        private void Write(string text, bool clear = false)
        {
            if (clear)
            {
                textBox1.Clear();
            }
            if (!string.IsNullOrEmpty(text))
            {
                textBox1.AppendText(string.Format("{0}{1}", text, Environment.NewLine));
            }
        }

        private void Write(Exception ex)
        {
            textBox1.Clear();
            textBox1.Text = ex.ToString();
        }

        private void GetVpnDetails(IEnumerable<XElement> vpnList)
        {
            foreach (var vpn in vpnList)
            {
                var data = new VpnData();
                data.Name = vpn.Attribute("name").Value;
                data.HostName = vpn.Attribute("vpnhost").Value;
                data.Username = vpn.Attribute("username").Value;
                data.Password = _securityManager.Decrypt(SecurityManager.Base64Decode(vpn.Attribute("password").Value));

                listBox1.Items.Add(data);
            }
            listBox1.DisplayMember = "Display";

            Write(string.Format("{0} vpn details found.", listBox1.Items.Count));
        }

        private void ConnectVpn2(VpnData data)
        {
            try
            {
                VpnSessionHost host = new VpnSessionHost(data);
                host.Connect(new NetworkCredential(data.Username, data.Password), data.HostName);
            }
            catch (Exception ex)
            {
                Write("Failed to connect, make sure you read the README.md file");
                Write(ex);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var data = listBox1.SelectedItem as VpnData;
            ConnectVpn2(data);
        }

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            button1_Click(sender, e);
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadConfig();
        }

        private void editConfigxmlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("config.xml");
        }

        private void editConnectionButton_Click(object sender, EventArgs e)
        {
            // Show the edit form
            var data = listBox1.SelectedItem as VpnData;
            EditConnectionForm editForm = new EditConnectionForm();
            editForm.VpnData = data;
            editForm.ShowDialog();

            // Replace the item with the new connection
            int index = listBox1.SelectedIndex;
            listBox1.Items.RemoveAt(index);
            listBox1.Items.Insert(index, editForm.VpnData);
            Save();
        }

        private void removeConnectionButton_Click(object sender, EventArgs e)
        {
            listBox1.Items.Remove(listBox1.SelectedItem as VpnData);
            listBox1.Refresh();
            Save();
        }

        private void Save()
        {
            XDocument configDoc = new XDocument();
            var rootEl = new XElement("vpnlist");
            configDoc.Add(rootEl);
            List<VpnData> vpns = (from object item in listBox1.Items select item as VpnData).ToList();
            foreach (VpnData vpn in vpns)
            {
                rootEl.Add(new XElement("vpn",
                    new XAttribute("name", vpn.Name),
                    new XAttribute("vpnhost", vpn.HostName),
                    new XAttribute("username", vpn.Username),
                    new XAttribute("password", SecurityManager.Base64Encode(_securityManager.Encrypt(vpn.Password)))));
            }
            configDoc.Save("Config.xml", SaveOptions.None);
        }

        private void addConnectionButton_Click(object sender, EventArgs e)
        {
            // Show the edit form
            AddConnectionForm addForm = new AddConnectionForm();
            addForm.ShowDialog();

            // Replace the item with the new connection
            listBox1.Items.Add(addForm.VpnData);
            Save();
        }
    }
}