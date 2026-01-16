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
    public partial class FrmShowAdmins : Form
    {

        string connectionString = "data source=SOAIBS-LAPTOP\\SQLEXPRESS; database=Project; integrated security=SSPI";
        public FrmShowAdmins()
        {
            InitializeComponent();
            LoadAdminData();
        }

        private void LoadAdminData()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                // Select all columns from the Admin table
                string query = "SELECT AdminId, Name, LoginId FROM Admin";
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                // Set the DataSource for the GridView
                dataGridView1.DataSource = dt;

                // Improve UI by making columns fill the width
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
        }
    }
}
