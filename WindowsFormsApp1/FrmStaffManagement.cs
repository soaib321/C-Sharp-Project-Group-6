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
    public partial class FrmStaffManagement : Form
    {
        string connectionString = "data source=SOAIBS-LAPTOP\\SQLEXPRESS; database=Project; integrated security=SSPI";
        public FrmStaffManagement()
        {
            InitializeComponent();
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.ReadOnly = true;
            string query = "SELECT * FROM [Staff]";
            FillDataGridView(query);
        }
        private void FillDataGridView(string query)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, con))
                {
                    con.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    DataTable dataTable = new DataTable();
                    dataTable.Load(reader);
                    dataGridView1.DataSource = dataTable;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtStaffId.Text))
            {
                MessageBox.Show("Please select a staff member from the list first.");
                return;
            }

            // 2. Define the Update Query
            // We update everything EXCEPT S_Id and LoginId as requested
            string query = @"UPDATE [Staff] 
                     SET Name = @name, 
                         Phone = @phone, 
                         S_Type = @type, 
                         Status = @status 
                     WHERE S_Id = @id";

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        // 3. Add parameters with values from your textboxes/comboboxes
                        cmd.Parameters.AddWithValue("@name", txtName.Text);
                        cmd.Parameters.AddWithValue("@phone", txtPhone.Text);
                        cmd.Parameters.AddWithValue("@type", cmbStaffType.Text);
                        cmd.Parameters.AddWithValue("@status", cmbStatus.Text);
                        cmd.Parameters.AddWithValue("@id", txtStaffId.Text);

                        con.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Staff details updated successfully!");

                            // 4. Refresh the DataGridView to show changes
                            FillDataGridView("SELECT * FROM [Staff]");
                        }
                        else
                        {
                            MessageBox.Show("Update failed. Record not found.");
                        }
                    }
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

                txtStaffId.Text = row.Cells["S_Id"].Value?.ToString() ?? "";
                txtName.Text = row.Cells["Name"].Value?.ToString() ?? "";
                txtPhone.Text = row.Cells["Phone"].Value?.ToString() ?? "";
                txtLoginId.Text = row.Cells["LoginId"].Value?.ToString() ?? "";

                cmbStaffType.Text = row.Cells["S_Type"].Value?.ToString() ?? "";
                cmbStatus.Text = row.Cells["Status"].Value?.ToString() ?? "";
            }
        }
        private void ClearFields()
        {
            txtStaffId.Clear();
            txtName.Clear();
            txtPhone.Clear();
            txtLoginId.Clear();
            cmbStaffType.SelectedIndex = -1;
            cmbStatus.SelectedIndex = -1;
        }

        private void deleteBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtStaffId.Text))
            {
                MessageBox.Show("Please select a staff member from the list to delete.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Ask for user confirmation
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete this staff member? This action cannot be undone.",
                                                        "Confirm Deletion",
                                                        MessageBoxButtons.YesNo,
                                                        MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                string query = "DELETE FROM [Staff] WHERE S_Id = @id";

                try
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            cmd.Parameters.AddWithValue("@id", txtStaffId.Text);

                            con.Open();
                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Staff record deleted successfully.");

                                // 3. Clear the textboxes and refresh the grid
                                ClearFields();
                                FillDataGridView("SELECT * FROM [Staff]");
                            }
                            else
                            {
                                MessageBox.Show("Delete failed. The record might have already been removed.");
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    // Note: If this staff ID is used in other tables (Foreign Keys), 
                    // the delete might fail. We handle that here.
                    MessageBox.Show("Database Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }
    }
}
