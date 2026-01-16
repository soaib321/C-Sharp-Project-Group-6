using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class UserRegistrationForm : Form
    {
        private Form loginForm; // FIX: store LoginForm reference

        // FIX: new constructor to accept LoginForm
        public UserRegistrationForm(Form login)
        {
            InitializeComponent();
            loginForm = login;
        }

        // Keep your existing default constructor if needed
        public UserRegistrationForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string name = textBox1.Text.Trim();
            string phone = textBox2.Text.Trim();
            string address = textBox3.Text.Trim();
            string username = textBox4.Text.Trim();
            string password = textBox5.Text.Trim();

            // 1️⃣ Validation
            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(phone) ||
                string.IsNullOrWhiteSpace(address) ||
                string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("All fields must be filled out.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string connectionString = "Data Source=SOAIBS-LAPTOP\\SQLEXPRESS;Database=Project;Integrated Security=SSPI";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                // 2️⃣ Check if username exists
                string checkUsernameQuery = "SELECT COUNT(*) FROM Login WHERE Username = @u";
                SqlCommand checkUsernameCmd = new SqlCommand(checkUsernameQuery, con);
                checkUsernameCmd.Parameters.AddWithValue("@u", username);
                int usernameExists = Convert.ToInt32(checkUsernameCmd.ExecuteScalar());

                if (usernameExists > 0)
                {
                    MessageBox.Show("Username already exists. Try another.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 2️⃣a Check if phone number exists
                string checkPhoneQuery = "SELECT COUNT(*) FROM [User] WHERE Phone = @ph";
                SqlCommand checkPhoneCmd = new SqlCommand(checkPhoneQuery, con);
                checkPhoneCmd.Parameters.AddWithValue("@ph", phone);
                int phoneExists = Convert.ToInt32(checkPhoneCmd.ExecuteScalar());

                if (phoneExists > 0)
                {
                    MessageBox.Show("Phone number already exists. Use a different phone.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 3️⃣ Insert into Login table (LoginId is IDENTITY)
                string loginQuery = "INSERT INTO Login (Username, Password, Role) OUTPUT INSERTED.LoginId " +
                                    "VALUES (@u, @p, 'User')";
                SqlCommand cmdLogin = new SqlCommand(loginQuery, con);
                cmdLogin.Parameters.AddWithValue("@u", username);
                cmdLogin.Parameters.AddWithValue("@p", password);

                object result = cmdLogin.ExecuteScalar();

                if (result != null)
                {
                    int loginId = Convert.ToInt32(result);

                    // 4️⃣ Insert into User table (UserId is IDENTITY)
                    string userQuery = "INSERT INTO [User] (Name, Phone, Address, LoginId) " +
                                       "VALUES (@n, @ph, @ad, @lid)";
                    SqlCommand cmdUser = new SqlCommand(userQuery, con);
                    cmdUser.Parameters.AddWithValue("@n", name);
                    cmdUser.Parameters.AddWithValue("@ph", phone);
                    cmdUser.Parameters.AddWithValue("@ad", address);
                    cmdUser.Parameters.AddWithValue("@lid", loginId);

                    int rowsAffected = cmdUser.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Registration successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        this.Close();             // Close registration form
                        if (loginForm != null)    // FIX: Show LoginForm safely
                            loginForm.Show();
                    }
                    else
                    {
                        MessageBox.Show("Failed to create profile. Try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Failed to insert login. Try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void UserRegistrationForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (loginForm != null)   // FIX: safely show LoginForm when registration is closed
            {
                loginForm.Show();
            }
        }

    }
}
           
    
            
    
    
            
    
    

