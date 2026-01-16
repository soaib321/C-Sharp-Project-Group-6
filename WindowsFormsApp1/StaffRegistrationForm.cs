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
    public partial class StaffRegistrationForm : Form
    {
        private Form loginForm;
        string connectionString = "Data Source=SOAIBS-LAPTOP\\SQLEXPRESS;Database=Project;Integrated Security=SSPI";
        public StaffRegistrationForm(Form login)
        {
            InitializeComponent();
            loginForm = login;
        }

        private void button1_Click(object sender, EventArgs e)
        {   // Gather inputs
            string name = textBox1.Text.Trim();
            string phone = textBox2.Text.Trim();
            string staffType = comboBox1.Text.Trim(); // Using ComboBox as requested
            string username = textBox4.Text.Trim();
            string password = textBox5.Text.Trim();

            // 1️⃣ Validation
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(phone) ||
                string.IsNullOrWhiteSpace(staffType) || string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("All fields must be filled out.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();

                    // 2️⃣ Check if username exists in Login table
                    string checkUsernameQuery = "SELECT COUNT(*) FROM Login WHERE Username = @u";
                    SqlCommand checkUsernameCmd = new SqlCommand(checkUsernameQuery, con);
                    checkUsernameCmd.Parameters.AddWithValue("@u", username);
                    int usernameExists = Convert.ToInt32(checkUsernameCmd.ExecuteScalar());

                    if (usernameExists > 0)
                    {
                        MessageBox.Show("Username already exists. Try another.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // 3️⃣ Insert into Login table first
                    // We must use OUTPUT INSERTED.LoginId to get the ID created by the database
                    string loginQuery = @"INSERT INTO Login (Username, Password, Role) 
                                          OUTPUT INSERTED.LoginId 
                                          VALUES (@u, @p, 'Staff')";

                    SqlCommand cmdLogin = new SqlCommand(loginQuery, con);
                    cmdLogin.Parameters.AddWithValue("@u", username);
                    cmdLogin.Parameters.AddWithValue("@p", password);

                    // ExecuteScalar returns the new LoginId
                    object result = cmdLogin.ExecuteScalar();

                    if (result != null)
                    {
                        int newLoginId = Convert.ToInt32(result);

                        // 4️⃣ Insert into Staff table using the new LoginId and 'Inactive' status
                        string staffQuery = @"INSERT INTO Staff (Name, Phone, S_Type, LoginId, Status) 
                                              VALUES (@n, @ph, @stype, @lid, @status)";

                        SqlCommand cmdStaff = new SqlCommand(staffQuery, con);
                        cmdStaff.Parameters.AddWithValue("@n", name);
                        cmdStaff.Parameters.AddWithValue("@ph", phone);
                        cmdStaff.Parameters.AddWithValue("@stype", staffType);
                        cmdStaff.Parameters.AddWithValue("@lid", newLoginId);
                        cmdStaff.Parameters.AddWithValue("@status", "Inactive"); // Initial state

                        int rowsAffected = cmdStaff.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Registration successful! Your account is 'Inactive' until approved by an Admin.",
                                            "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.Close();
                        }
                    }
                }
                catch (SqlException sqlEx)
                {
                    MessageBox.Show("Database Error: " + sqlEx.Message, "SQL Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void StaffRegistrationForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (loginForm != null)
                loginForm.Show();
        }
    }
    }

