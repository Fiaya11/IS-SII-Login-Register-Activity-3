using System;
using System.Windows.Forms;

namespace SofiaFe_Act2
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
            button1.Click += button1_Click;
            button2.Click += button2_Click;
            AcceptButton = button1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string username = maskedTextBox1.Text.Trim();
            string password = maskedTextBox2.Text;

            if (username.Length == 0 || password.Length == 0)
            {
                MessageBox.Show("Please enter your username and password.", "Missing information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                UserAccount account = Database.Authenticate(username, password);
                if (account == null)
                {
                    MessageBox.Show("The username or password is incorrect.", "Login failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    maskedTextBox2.Clear();
                    maskedTextBox2.Focus();
                    return;
                }

                Hide();
                using (Dashboard dashboard = new Dashboard(account))
                {
                    dashboard.ShowDialog();
                }
                maskedTextBox2.Clear();
                Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Database connection", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (SignUp signUp = new SignUp())
            {
                Hide();
                signUp.ShowDialog();

                if (signUp.CreatedAccount != null)
                {
                    using (Dashboard dashboard = new Dashboard(signUp.CreatedAccount))
                    {
                        dashboard.ShowDialog();
                    }
                }

                Show();
            }
            maskedTextBox1.Focus();
        }
    }
}
