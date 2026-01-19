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
    public partial class allreq : Form
    {
        private SqlConnection con = new SqlConnection("data source=SOAIBS-LAPTOP\\SQLEXPRESS; database=Project; integrated security=SSPI");
        private int loggedInUserId;

        public allreq()
        {
            InitializeComponent();
        }

        public allreq(int userId)
        {
            InitializeComponent();
            loggedInUserId = userId;
        }

        private void allreq_Load(object sender, EventArgs e)
        {
            LoadAllRequests();
        }

        private void LoadAllRequests()
        {
            try
            {
                string query = @"
                    SELECT CAST(W_ReqId AS NVARCHAR(50)) AS ID, W_Type AS [Type], CAST(Quantity AS NVARCHAR(50)) AS Quantity, Req_Date AS [Date], Status
                    FROM W_Req
                    WHERE UserId = @UserId

                    UNION ALL

                    SELECT CAST(C_ReqId AS NVARCHAR(50)), C_Type, 'N/A', C_Date, Status
                    FROM C_Req
                    WHERE UserId = @UserId

                    UNION ALL

                    SELECT CAST(P_ReqId AS NVARCHAR(50)), PestType, CAST(CoverageArea AS NVARCHAR(50)), RequestDate, Status
                    FROM Pes_Req
                    WHERE UserId = @UserId

                    ORDER BY [Date] DESC";

                SqlCommand cmd = new SqlCommand(query, con);
               
                cmd.Parameters.AddWithValue("@UserId", loggedInUserId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dataGridView1.DataSource = dt;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                dataGridView1.Columns["ID"].HeaderText = "Request ID";
                dataGridView1.Columns["Type"].HeaderText = "Request Type";
                dataGridView1.Columns["Quantity"].HeaderText = "Qty / Area";
                dataGridView1.Columns["Date"].HeaderText = "Date Submitted";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading combined requests: " + ex.Message);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Userdashboard dashboard = new Userdashboard();
            dashboard.Show();
            this.Close();
        }
    }
}





