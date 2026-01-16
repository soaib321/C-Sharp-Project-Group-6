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
        public allreq()
        {
            InitializeComponent();
        }
        private SqlConnection con = new SqlConnection("data source=SOAIBS-LAPTOP\\SQLEXPRESS; database=Project; integrated security=SSPI");
        private int loggedInUserId; 

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
                string query = @"SELECT W_ReqId, W_Type, Quantity, Req_Date, Status
                                 FROM W_Req
                                 WHERE UserId = @UserId
                                 ORDER BY Req_Date DESC";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@UserId", loggedInUserId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dataGridView1.DataSource = dt;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading requests: " + ex.Message);
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


   


