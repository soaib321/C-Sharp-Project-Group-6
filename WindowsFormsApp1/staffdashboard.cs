using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class staffdashboard : Form
    {
        private readonly string cs = "Data Source=SOAIBS-LAPTOP\\SQLEXPRESS;Initial Catalog=Project;Integrated Security=SSPI";

        private int loggedInStaffId;
        private int loginId;

        public staffdashboard(int loginId)
        {
            InitializeComponent();
            this.loginId = loginId;

            dataGridView1.CellClick += dataGridView1_CellClick;
            dataGridView2.CellClick += dataGridView2_CellClick;
        }

        private void staffdashboard_Load(object sender, EventArgs e)
        {
            loggedInStaffId = GetStaffIdFromLogin(loginId);

            if (loggedInStaffId == 0)
            {
                MessageBox.Show("Staff not found.");
                Close();
                return;
            }

            LoadAllPendingTasks();
            LoadAllMyAcceptedTasks();
        }

        // ================= STAFF ID =================
        private int GetStaffIdFromLogin(int loginId)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = "SELECT S_Id FROM Staff WHERE LoginId=@id";
                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.AddWithValue("@id", loginId);

                con.Open();
                object r = cmd.ExecuteScalar();
                return r == null ? 0 : Convert.ToInt32(r);
            }
        }

        // ================= AVAILABLE TASKS =================
        private void LoadAllPendingTasks()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
          SELECT 
                  CAST(W_ReqId AS NVARCHAR(50)) AS ReqId,
                    'Waste' AS TaskType,
                     W_Type AS Description,
                     CAST(Quantity AS NVARCHAR(50)) AS Info,
                     Req_Date AS ReqDate,
                     Status 
                     FROM W_Req
                     WHERE Status = 'Pending'

                     UNION ALL

                     SELECT 
                     CAST(C_ReqId AS NVARCHAR(50)),
                     'Cleaning',
                      C_Type,
    CAST(NULL AS NVARCHAR(50)),
    C_Date,
    Status
FROM C_Req
WHERE Status = 'Pending'

UNION ALL

SELECT 
                     P_ReqId,
                   'Pest Control',
                    PestType,
                  CoverageArea, 
    RequestDate,
                   Status 
                FROM Pes_Req
           WHERE Status = 'Pending'";


                SqlDataAdapter da = new SqlDataAdapter(q, con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dataGridView1.Columns.Clear();
                dataGridView1.DataSource = dt;

                AddAcceptButton();
            }
        }

        private void AddAcceptButton()
        {
            if (!dataGridView1.Columns.Contains("Accept"))
            {
                DataGridViewButtonColumn btn = new DataGridViewButtonColumn();
                btn.Name = "Accept";
                btn.HeaderText = "Action";
                btn.Text = "Accept";
                btn.UseColumnTextForButtonValue = true;
                dataGridView1.Columns.Add(btn);
            }
        }

        // ================= ACCEPTED TASKS =================
        private void LoadAllMyAcceptedTasks()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
SELECT 
    CAST(W_ReqId AS NVARCHAR(50)) AS ReqId,
    'Waste' AS TaskType,
    W_Type AS Description,
    CAST(Quantity AS NVARCHAR(50)),
    Req_Date AS ReqDate,
    Status
FROM W_Req
WHERE Status = 'Accepted' AND S_Id = @sid

UNION ALL

SELECT 
    CAST(C_ReqId AS NVARCHAR(50)),
    'Cleaning',
    C_Type,
    NULL,
    C_Date,
    Status
FROM C_Req
WHERE Status = 'Accepted' AND S_Id = @sid

UNION ALL

SELECT 
    P_ReqId,
    'Pest Control',
    PestType,
    CoverageArea,
    RequestDate,
    Status
FROM Pes_Req
WHERE Status = 'Accepted' AND S_Id = @sid";


                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.AddWithValue("@sid", loggedInStaffId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dataGridView2.Columns.Clear();
                dataGridView2.DataSource = dt;

                AddCompleteButton();
            }
        }

        private void AddCompleteButton()
        {
            if (!dataGridView2.Columns.Contains("Complete"))
            {
                DataGridViewButtonColumn btn = new DataGridViewButtonColumn();
                btn.Name = "Complete";
                btn.HeaderText = "Action";
                btn.Text = "Complete";
                btn.UseColumnTextForButtonValue = true;
                dataGridView2.Columns.Add(btn);
            }
        }

        // ================= GRID ACTIONS =================
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (dataGridView1.Columns[e.ColumnIndex].Name == "Accept")
            {
                var id = dataGridView1.Rows[e.RowIndex].Cells["ReqId"].Value;
                string taskType = dataGridView1.Rows[e.RowIndex].Cells["TaskType"].Value.ToString();

                switch (taskType)
                {
                    case "Waste": AcceptWasteRequest(Convert.ToInt32(id)); break;
                    case "Cleaning": AcceptCleaningRequest(Convert.ToInt32(id)); break;
                    case "Pest Control": AcceptPestRequest(id.ToString()); break;
                }

                LoadAllPendingTasks();
                LoadAllMyAcceptedTasks();
            }
        }

        private void AcceptWasteRequest(int id)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"UPDATE W_Req SET Status='Accepted', S_Id=@sid WHERE W_ReqId=@id";
                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.AddWithValue("@sid", loggedInStaffId);
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private void AcceptCleaningRequest(int id)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"UPDATE C_Req SET Status='Accepted', S_Id=@sid WHERE C_ReqId=@id";
                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.AddWithValue("@sid", loggedInStaffId);
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private void AcceptPestRequest(string id)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"UPDATE Pes_Req SET Status='Accepted', S_Id=@sid WHERE P_ReqId=@id";
                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.AddWithValue("@sid", loggedInStaffId);
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (dataGridView2.Columns[e.ColumnIndex].Name == "Complete")
            {
                var id = dataGridView2.Rows[e.RowIndex].Cells["ReqId"].Value;
                string taskType = dataGridView2.Rows[e.RowIndex].Cells["TaskType"].Value.ToString();

                switch (taskType)
                {
                    case "Waste": CompleteWasteRequest(Convert.ToInt32(id)); break;
                    case "Cleaning": CompleteCleaningRequest(Convert.ToInt32(id)); break;
                    case "Pest Control": CompletePestRequest(id.ToString()); break;
                }

                LoadAllMyAcceptedTasks();
            }
        }

        private void CompleteWasteRequest(int id)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = "UPDATE W_Req SET Status='Completed' WHERE W_ReqId=@id";
                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private void CompleteCleaningRequest(int id)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = "UPDATE C_Req SET Status='Completed' WHERE C_ReqId=@id";
                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private void CompletePestRequest(string id)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = "UPDATE Pes_Req SET Status='Completed' WHERE P_ReqId=@id";
                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }




 //================= BUTTONS =================
private void button2_Click(object sender, EventArgs e)
        {
            staffprofile a = new staffprofile( loginId);
            this.Close();
            a.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to logout?", "Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {

                LoginForm login = new LoginForm();
                login.Show();

                this.Close();
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            LoadAllMyAcceptedTasks();
        }
        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Forward to the actual handler
            dataGridView2_CellClick(sender, e);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
