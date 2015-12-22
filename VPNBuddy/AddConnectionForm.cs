using System.Windows.Forms;

namespace VPNBuddy
{
    public partial class AddConnectionForm : Form
    {
        public AddConnectionForm()
        {
            InitializeComponent();
        }

        public VpnData VpnData
        {
            get
            {
                return new VpnData
                {
                    HostName = urlTextBox.Text,
                    Password = passwordTextBox.Text,
                    Username = userTextBox.Text,
                    Name = nameTextBox.Text
                };
            }
            set
            {
                nameTextBox.Text = value.Name;
                passwordTextBox.Text = value.Password;
                userTextBox.Text = value.Username;
                urlTextBox.Text = value.HostName;
            } 
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            Close();
        }
    }
}
