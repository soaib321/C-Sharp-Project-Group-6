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
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            textBox2.UseSystemPasswordChar = !checkBox1.Checked;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string username = textBox1.Text.Trim();
            string password = textBox2.Text.Trim();

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("All fields must be filled out.", "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string connectionString = "Data Source=SOAIBS-LAPTOP\\SQLEXPRESS;Database=Project;Integrated Security=SSPI";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();

                    // Select LoginId and Role instead of COUNT(*) 
                    string query = @"
                SELECT L.LoginId, L.Role, S.Status 
                FROM Login L
                LEFT JOIN Staff S ON L.LoginId = S.LoginId
                WHERE LTRIM(RTRIM(L.Username)) COLLATE SQL_Latin1_General_CP1_CS_AS = @u
                  AND LTRIM(RTRIM(L.Password)) COLLATE SQL_Latin1_General_CP1_CS_AS = @p";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@u", username);
                    cmd.Parameters.AddWithValue("@p", password);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read()) // If a record is found
                    {
                        int loginId = Convert.ToInt32(reader["LoginId"]);
                        string role = reader["Role"].ToString();
                        string status = reader["Status"] != DBNull.Value ? reader["Status"].ToString() : "Active";

                        MessageBox.Show($"Login successful! Role: {role}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        this.Hide();

                        // Redirect based on Role
                        if (role == "Admin")
                        {
                            // Pass the LoginId to the Admin Dashboard constructor
                            FrmAdminDashboard adminDash = new FrmAdminDashboard(loginId);
                            adminDash.Show();
                        }
                        else if (role == "Staff" && status != "Active")
                        {
                            MessageBox.Show("This account is not verified yet.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            return; // Stop the login process here
                        }
                        else if (role == "User")
                        {
                            Userdashboard userDash = new Userdashboard(loginId);
                            userDash.Show();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Invalid username or password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Open RoleSelectionForm and pass LoginForm instance
            RoleSelectionForm roleForm = new RoleSelectionForm(this);
            roleForm.Show();

            this.Hide(); // hide login while role selection is open
        }


    }
}



    
