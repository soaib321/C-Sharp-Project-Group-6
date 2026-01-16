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
    public partial class FrmChangePass : Form
    {
        string connectionString = "data source=SOAIBS-LAPTOP\\SQLEXPRESS; database=Project; integrated security=SSPI";
        int adminId;

        public FrmChangePass(int id)
        {
            adminId = id;
            InitializeComponent();
        }
        public FrmChangePass()
        {
            InitializeComponent();
        }

        private void confirmBtn_Click(object sender, EventArgs e)
        {
            string oldPass = oldPassTxt.Text; // Ensure these names match your TextBox names
            string newPass = newPassTxt.Text;

            if (string.IsNullOrEmpty(oldPass) || string.IsNullOrEmpty(newPass))
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();

                    // 1. Find the LoginId associated with this Admin and verify current password
                    string verifyQuery = @"
                        SELECT L.LoginId 
                        FROM Login L 
                        JOIN Admin A ON L.LoginId = A.LoginId 
                        WHERE A.AdminId = @adminId AND L.Password = @oldPass";

                    SqlCommand cmd = new SqlCommand(verifyQuery, con);
                    cmd.Parameters.AddWithValue("@adminId", adminId);
                    cmd.Parameters.AddWithValue("@oldPass", oldPass);

                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        int loginId = (int)result;

                        // 2. Update the password in the Login table
                        string updateQuery = "UPDATE Login SET Password = @newPass WHERE LoginId = @loginId";
                        SqlCommand updateCmd = new SqlCommand(updateQuery, con);
                        updateCmd.Parameters.AddWithValue("@newPass", newPass);
                        updateCmd.Parameters.AddWithValue("@loginId", loginId);

                        updateCmd.ExecuteNonQuery();
                        MessageBox.Show("Password changed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Incorrect old password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }

        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            this.Close();

        }
    }
}
