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
        public FrmUserManagement()
        {
            InitializeComponent();
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.ReadOnly = true;
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

        }
    }
}
