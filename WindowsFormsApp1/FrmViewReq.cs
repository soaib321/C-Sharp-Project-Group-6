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
    public partial class FrmViewReq : Form
    {
        string connectionString = "data source=SOAIBS-LAPTOP\\SQLEXPRESS; database=Project; integrated security=SSPI";
        public FrmViewReq()
        {
            InitializeComponent();
            LoadAllRequests();
        }
        private void LoadAllRequests(string filter = "")
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    // The UNION query merges the three request tables based on common fields
                    string query = @"
                        SELECT 'Cleaning' AS Category, C_ReqId AS ID, UserId, S_Id, Status, C_Date AS ReqDate FROM C_Req
                        UNION ALL
                        SELECT 'Pest' AS Category, P_ReqId AS ID, UserId, S_Id, Status, RequestDate AS ReqDate FROM Pes_Req
                        UNION ALL
                        SELECT 'Waste' AS Category, W_ReqId AS ID, UserId, S_Id, Status, Req_Date AS ReqDate FROM W_Req";

                    // If searching, wrap the union in a subquery to filter everything at once
                    if (!string.IsNullOrEmpty(filter))
                    {
                        query = $"SELECT * FROM ({query}) AS AllReqs WHERE Category LIKE '%{filter}%' OR Status LIKE '%{filter}%' OR CAST(ID AS VARCHAR) LIKE '%{filter}%'";
                    }

                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dataGridView1.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message);
            }
        }
        private void searchBtn_Click(object sender, EventArgs e)
        {
            LoadAllRequests(textBox1.Text.Trim());

        }

        private void refreshBtn_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            LoadAllRequests();
        }
    }
}
