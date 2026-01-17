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
using System.Xml.Linq;

namespace WindowsFormsApp1
{
    public partial class FrmUserManagement : Form
    {
        string connectionString = "data source=SOAIBS-LAPTOP\\SQLEXPRESS; database=Project; integrated security=SSPI";
        int loginId;
        public FrmUserManagement(int loginId)
        {
            InitializeComponent();
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.ReadOnly = true;
            this.loginId = loginId;
            LoadUserData();
        }

        private void LoadUserData()
        {
            string query = "SELECT * FROM [User]";
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;
            }
        }

        private void updateBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtUserId.Text))
            {
                MessageBox.Show("Please select a user to update.");
                return;
            }

            string query = @"UPDATE [User] 
                             SET Name = @name, Phone = @phone, Address = @address 
                             WHERE UserId = @id";

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@name", txtName.Text);
                    cmd.Parameters.AddWithValue("@phone", txtPhone.Text);
                    cmd.Parameters.AddWithValue("@address", txtAddress.Text);
                    cmd.Parameters.AddWithValue("@id", txtUserId.Text);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("User updated successfully!");
                    LoadUserData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                txtUserId.Text = row.Cells["UserId"].Value?.ToString();
                txtName.Text = row.Cells["Name"].Value?.ToString();
                txtPhone.Text = row.Cells["Phone"].Value?.ToString();
                txtAddress.Text = row.Cells["Address"].Value?.ToString();
                txtLoginId.Text = row.Cells["LoginId"].Value?.ToString();
            }
        }

        private void deleteBtn_Click(object sender, EventArgs e)
        {
            // 1. Validation: Ensure a User ID is selected
            if (string.IsNullOrWhiteSpace(txtUserId.Text))
            {
                MessageBox.Show("Please select a user from the list to delete.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Confirmation Dialog
            DialogResult result = MessageBox.Show("Are you sure you want to delete this user? This action cannot be undone.",
                "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                string query = "DELETE FROM [User] WHERE UserId = @id";

                try
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            cmd.Parameters.AddWithValue("@id", txtUserId.Text);
                            con.Open();
                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("User deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                // Refresh UI
                                clrBtn_Click(sender, e);
                                LoadUserData();
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    // Error 547 is a Foreign Key constraint violation (e.g., user has active orders/requests)
                    if (ex.Number == 547)
                    {
                        MessageBox.Show("Cannot delete this user because they have active records (orders or requests) linked to their account.",
                            "Database Constraint", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show("Database Error: " + ex.Message, "SQL Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An unexpected error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void clrBtn_Click(object sender, EventArgs e)
        {
            // Clear all textboxes
            txtUserId.Clear();
            txtName.Clear();
            txtPhone.Clear();
            txtAddress.Clear();
            txtLoginId.Clear();

            // Reset focus to the Name field
            txtName.Focus();


        }

        private void backBtn_Click(object sender, EventArgs e)
        {
            this.Close();
            FrmAdminDashboard adminDashboard = new FrmAdminDashboard(loginId);
            adminDashboard.Show();

        }
    }
}
