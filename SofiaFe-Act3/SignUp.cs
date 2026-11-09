using System;
using System.Windows.Forms;

namespace SofiaFe_Act2
{
    public partial class SignUp : Form
    {
        public UserAccount CreatedAccount { get; private set; }

        public SignUp()
        {
            InitializeComponent();
            button1.Click += button1_Click;
            button2.Click += button2_Click;
            AcceptButton = button1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string fullName = maskedTextBox3.Text.Trim();
            string username = maskedTextBox1.Text.Trim();
            string password = maskedTextBox2.Text;

            if (fullName.Length == 0 || username.Length == 0 || password.Length == 0)
            {
                MessageBox.Show("Please complete all fields.", "Missing information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (username.Length > 50 || fullName.Length > 100)
            {
                MessageBox.Show("Full name must be 100 characters or fewer and username must be 50 characters or fewer.", "Input too long", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (password.Length < 6)
            {
                MessageBox.Show("Password must contain at least 6 characters.", "Weak password", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Database.CreateAccount(fullName, username, password);
                CreatedAccount = Database.Authenticate(username, password);
                MessageBox.Show("Your account has been created successfully.", "Registration complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Registration failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Database connection", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
