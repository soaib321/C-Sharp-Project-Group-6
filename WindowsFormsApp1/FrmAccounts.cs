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
        int aid;
        public FrmAccounts(int id)
        {
            InitializeComponent();
            aid = id;
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
            // --- 1. INPUT VALIDATION ---

            // Check for empty strings or whitespace
            if (string.IsNullOrWhiteSpace(txtUsername.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Text) ||
                string.IsNullOrWhiteSpace(cmbRole.Text))
            {
                MessageBox.Show("Please fill in Username, Password, and Role.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Example of specific validation: Username length
            if (txtUsername.Text.Length < 3)
            {
                MessageBox.Show("Username must be at least 3 characters long.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // --- 2. DATABASE EXECUTION ---
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    // NOTICE: Loginid is removed from the query because it is an IDENTITY column
                    string query = "INSERT INTO Login (Username, Password, Role) VALUES (@user, @pass, @role)";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@user", txtUsername.Text.Trim());
                    cmd.Parameters.AddWithValue("@pass", txtPassword.Text);
                    cmd.Parameters.AddWithValue("@role", cmbRole.Text);

                    con.Open();
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Account created successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Refresh UI
                    LoadData();
                    clrBtn_Click(sender, e);
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Database Error: " + ex.Message, "SQL Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An unexpected error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void updateBtn_Click(object sender, EventArgs e)
        {
            // 1. Validation: Ensure a record is selected (LoginId is present)
            if (string.IsNullOrWhiteSpace(txtLoginId.Text))
            {
                MessageBox.Show("Please select an account from the list to update.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Validation: Ensure fields aren't empty
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Username and Password cannot be empty.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    // Standard UPDATE query to modify existing record details
                    string query = "UPDATE Login SET Username = @user, Password = @pass, Role = @role WHERE Loginid = @id";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@user", txtUsername.Text.Trim());
                    cmd.Parameters.AddWithValue("@pass", txtPassword.Text);
                    cmd.Parameters.AddWithValue("@role", cmbRole.Text);
                    cmd.Parameters.AddWithValue("@id", txtLoginId.Text);

                    con.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Account updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Refresh UI
                        LoadData();
                        clrBtn_Click(sender, e);
                    }
                    else
                    {
                        MessageBox.Show("Update failed. The record might have been deleted.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void deleteBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtLoginId.Text))
            {
                MessageBox.Show("Please select a user to delete.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Data Type Validation: Ensure it's a valid number
            if (!int.TryParse(txtLoginId.Text, out int loginId))
            {
                MessageBox.Show("Invalid Login ID format. Please select a valid record.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // --- 3. DATABASE EXISTENCE CHECK ---
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string checkQuery = "SELECT COUNT(*) FROM Login WHERE Loginid = @id";
                    SqlCommand checkCmd = new SqlCommand(checkQuery, con);
                    checkCmd.Parameters.AddWithValue("@id", loginId);

                    int count = (int)checkCmd.ExecuteScalar();

                    if (count == 0)
                    {
                        MessageBox.Show("This account no longer exists in the database.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData(); // Refresh grid to show actual data
                        clrBtn_Click(sender, e);
                        return;
                    }

                    // --- 4. CONFIRMATION & DELETION LOGIC ---
                    string confirmMessage = "Are you sure you want to delete this account?";
                    if (cmbRole.Text.Trim().Equals("Admin", StringComparison.OrdinalIgnoreCase))
                    {
                        confirmMessage += " This will also remove them from the Admin records.";
                    }

                    DialogResult result = MessageBox.Show(confirmMessage, "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (result == DialogResult.Yes)
                    {
                        SqlTransaction transaction = con.BeginTransaction();
                        try
                        {
                            // Delete from Admin table first
                            string deleteAdminQuery = "DELETE FROM Admin WHERE LoginId = @id";
                            SqlCommand adminCmd = new SqlCommand(deleteAdminQuery, con, transaction);
                            adminCmd.Parameters.AddWithValue("@id", loginId);
                            adminCmd.ExecuteNonQuery();

                            // Delete from Login table
                            string deleteLoginQuery = "DELETE FROM Login WHERE Loginid = @id";
                            SqlCommand loginCmd = new SqlCommand(deleteLoginQuery, con, transaction);
                            loginCmd.Parameters.AddWithValue("@id", loginId);
                            loginCmd.ExecuteNonQuery();

                            transaction.Commit();

                            MessageBox.Show("Account deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Clean up UI
                            clrBtn_Click(sender, e);
                            LoadData();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            MessageBox.Show("Error during deletion: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Database connection error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void backBtn_Click(object sender, EventArgs e)
        {
            this.Close();
            FrmAdminDashboard f = new FrmAdminDashboard(aid);
            f.Show();


        }

        private void clrBtn_Click(object sender, EventArgs e)
        {
            txtLoginId.Clear();
            txtUsername.Clear();
            txtPassword.Clear();
            cmbRole.SelectedIndex = -1; // Deselects any selected item in the dropdown

            // Optional: Return focus to the first textbox
            txtLoginId.Focus();
        }
    }
}
