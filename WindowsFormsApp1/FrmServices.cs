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
    public partial class FrmServices : Form
    {
        string connectionString = "data source=SOAIBS-LAPTOP\\SQLEXPRESS; database=Project; integrated security=SSPI";
        public FrmServices()
        {
            InitializeComponent();
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.ReadOnly = true;
            LoadServices();
        }
        private void LoadServices()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM Service";
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridView1.DataSource = dt; // Assuming your grid is named dataGridView1
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading services: " + ex.Message);
            }
        }

        private void addBtn_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO Service (ServiceName, ServiceType, Price, IsActive) VALUES (@name, @type, @price, @active)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@name", txtServiceName.Text);
                    cmd.Parameters.AddWithValue("@type", cmbServiceType.Text);
                    cmd.Parameters.AddWithValue("@price", decimal.Parse(txtPrice.Text));
                    // Map "Active" text to boolean 1/True
                    cmd.Parameters.AddWithValue("@active", cmbStatus.Text == "Active" ? 1 : 0);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Service Added Successfully");
                    LoadServices();
                    ClearFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }


        }

        private void updateBtn_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                try
                {
                    int id = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["ServiceID"].Value);
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        string query = "UPDATE Service SET ServiceName=@name, ServiceType=@type, Price=@price, IsActive=@active WHERE ServiceID=@id";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@name", txtServiceName.Text);
                        cmd.Parameters.AddWithValue("@type", cmbServiceType.Text);
                        cmd.Parameters.AddWithValue("@price", decimal.Parse(txtPrice.Text));
                        cmd.Parameters.AddWithValue("@active", cmbStatus.Text == "Active" ? 1 : 0);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Service Updated Successfully");
                        LoadServices();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }

        }

        private void deleteBtn_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete this service?", "Delete", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    int id = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["ServiceID"].Value);
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        string query = "DELETE FROM Service WHERE ServiceID=@id";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@id", id);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                        LoadServices();
                    }
                }
            }

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                txtServiceName.Text = row.Cells["ServiceName"].Value.ToString();
                cmbServiceType.Text = row.Cells["ServiceType"].Value.ToString();
                txtPrice.Text = row.Cells["Price"].Value.ToString();
                // Check boolean and set dropdown text
                bool isActive = Convert.ToBoolean(row.Cells["IsActive"].Value);
                cmbStatus.Text = isActive ? "Active" : "Inactive";
            }

        }
        private void ClearFields()
        {
            txtServiceName.Clear();
            txtPrice.Clear();
            cmbServiceType.SelectedIndex = -1;
            cmbStatus.SelectedIndex = -1;
        }

        private void backBtn_Click(object sender, EventArgs e)
        {

        }

        private void FrmServices_Load(object sender, EventArgs e)
        {

        }
    }
}
