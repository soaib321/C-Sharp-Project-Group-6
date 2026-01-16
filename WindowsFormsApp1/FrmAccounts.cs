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
    public partial class FrmAccounts : Form
    {
        string connectionString = "data source=SOAIBS-LAPTOP\\SQLEXPRESS; database=Project; integrated security=SSPI";
        public FrmAccounts()
        {
            InitializeComponent();
            LoadData();
        }
        private void LoadData()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Login", con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;
            }
        }

        private void addBtn_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Login (Loginid, Username, Password, Role) VALUES (@id, @user, @pass, @role)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", txtLoginId.Text); // Replace with your actual TextBox names
                cmd.Parameters.AddWithValue("@user", txtUsername.Text);
                cmd.Parameters.AddWithValue("@pass", txtPassword.Text);
                cmd.Parameters.AddWithValue("@role", cmbRole.Text);

                con.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("Account Added Successfully");
                LoadData();
            }

        }

        private void updateBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtLoginId.Text))
            {
                MessageBox.Show("Please select a user to promote.");
                return;
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                // A transaction ensures both tables update or neither does
                SqlTransaction transaction = con.BeginTransaction();

                try
                {
                    // 1. Update the Role in dbo.Login to 'Admin'
                    string updateQuery = "UPDATE Login SET Role = 'Admin' WHERE Loginid = @id";
                    SqlCommand updateCmd = new SqlCommand(updateQuery, con, transaction);
                    updateCmd.Parameters.AddWithValue("@id", txtLoginId.Text);
                    updateCmd.ExecuteNonQuery();

                    // 2. Insert into dbo.Admin
                    // We omit AdminId because it is now an Auto-Increment Identity column
                    string insertQuery = "INSERT INTO Admin (Name, LoginId) VALUES (@name, @loginId)";
                    SqlCommand insertCmd = new SqlCommand(insertQuery, con, transaction);
                    insertCmd.Parameters.AddWithValue("@name", txtUsername.Text); // From your Username textbox
                    insertCmd.Parameters.AddWithValue("@loginId", txtLoginId.Text); // Linking the two tables
                    insertCmd.ExecuteNonQuery();

                    // Commit changes if both commands succeed
                    transaction.Commit();
                    MessageBox.Show("User successfully promoted to Admin!");

                    LoadData(); // Refresh your DataGridView to show the new Role
                }
                catch (Exception ex)
                {
                    // Rollback changes if an error occurs (e.g., database connection lost)
                    transaction.Rollback();
                    MessageBox.Show("Error during promotion: " + ex.Message);
                }
            }

        }

        private void deleteBtn_Click(object sender, EventArgs e)
        {
            // Ensure a user is selected before attempting deletion
            if (string.IsNullOrEmpty(txtLoginId.Text))
            {
                MessageBox.Show("Please select a user to delete.");
                return;
            }

            // Confirmation dialog to prevent accidental deletion
            DialogResult result = MessageBox.Show("Are you sure you want to delete this account? This will also remove them from the Admin records.", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlTransaction transaction = con.BeginTransaction();

                    try
                    {
                        // 1. Delete from Admin table first (to handle potential foreign key constraints)
                        string deleteAdminQuery = "DELETE FROM Admin WHERE LoginId = @id";
                        SqlCommand adminCmd = new SqlCommand(deleteAdminQuery, con, transaction);
                        adminCmd.Parameters.AddWithValue("@id", txtLoginId.Text);
                        adminCmd.ExecuteNonQuery();

                        // 2. Delete from Login table
                        string deleteLoginQuery = "DELETE FROM Login WHERE Loginid = @id";
                        SqlCommand loginCmd = new SqlCommand(deleteLoginQuery, con, transaction);
                        loginCmd.Parameters.AddWithValue("@id", txtLoginId.Text);
                        loginCmd.ExecuteNonQuery();

                        // Commit both deletions
                        transaction.Commit();

                        MessageBox.Show("Account and associated Admin records deleted successfully.");

                        // Clear textboxes after deletion
                        txtLoginId.Clear();
                        txtUsername.Clear();
                        txtPassword.Clear();
                        cmbRole.SelectedIndex = -1;

                        LoadData(); // Refresh the DataGridView
                    }
                    catch (Exception ex)
                    {
                        // If any part fails, roll back both deletions
                        transaction.Rollback();
                        MessageBox.Show("Error during deletion: " + ex.Message);
                    }
                }
            }
        }

        private void promoteBtn_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(txtLoginId.Text))
            {
                MessageBox.Show("Please select a user first.");
                return;
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlTransaction transaction = con.BeginTransaction();

                try
                {
                    // 1. Update Role in Login table
                    string updateQuery = "UPDATE Login SET Role = 'Admin' WHERE Loginid = @id";
                    SqlCommand updateCmd = new SqlCommand(updateQuery, con, transaction);
                    updateCmd.Parameters.AddWithValue("@id", txtLoginId.Text);
                    updateCmd.ExecuteNonQuery();

                    // 2. Insert into Admin table (AdminId is omitted because it's now an Identity)
                    string insertQuery = "INSERT INTO Admin (Name, LoginId) VALUES (@name, @loginId)";
                    SqlCommand insertCmd = new SqlCommand(insertQuery, con, transaction);
                    insertCmd.Parameters.AddWithValue("@name", txtUsername.Text);
                    insertCmd.Parameters.AddWithValue("@loginId", txtLoginId.Text);
                    insertCmd.ExecuteNonQuery();

                    transaction.Commit();
                    MessageBox.Show("Promotion Successful!");
                    LoadData(); // Refresh your grid
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Error: " + ex.Message);
                }
            }

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                txtLoginId.Text = row.Cells["Loginid"].Value.ToString();
                txtUsername.Text = row.Cells["Username"].Value.ToString();
                txtPassword.Text = row.Cells["Password"].Value.ToString();
                cmbRole.Text = row.Cells["Role"].Value.ToString();
            }

        }
    }
}
