using System;
using System.Drawing;
using System.Windows.Forms;

namespace SofiaFe_Act2
{
    public partial class Dashboard : Form
    {
        private readonly UserAccount account;

        public Dashboard(UserAccount currentAccount)
        {
            account = currentAccount;
            InitializeComponent();
            welcomeLabel.Text = "Welcome, " + account.FullName + "!";
            logoutButton.Click += logoutButton_Click;
        }

        private void logoutButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Dashboard_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
