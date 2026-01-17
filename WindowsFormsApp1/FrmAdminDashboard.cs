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
    public partial class FrmAdminDashboard : Form
    {
        string connectionString = "Data Source=SOAIBS-LAPTOP\\SQLEXPRESS;Database=Project;Integrated Security=SSPI";
        int loginId;
        int idt;
        public FrmAdminDashboard()
        {
            InitializeComponent();
        }
        public FrmAdminDashboard(int id)
        {
            loginId = id;
            InitializeComponent();
            PopulateAdminDetails();
        }

        private void PopulateAdminDetails()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    // Query to find Admin details linked to the LoginId
                    string query = "SELECT AdminId, Name FROM Admin WHERE LoginId = @loginId";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@loginId", loginId);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        // Assuming your TextBoxes are named txtAdminName and txtAdminID
                        txtAdminName.Text = reader["Name"].ToString();
                        txtAdminID.Text = reader["AdminId"].ToString();
                        idt = Convert.ToInt32(reader["AdminId"]);
                    }
                    else
                    {
                        MessageBox.Show("Admin profile not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Database Error: " + ex.Message);
                }
            }
        }

        private void accountBtn_Click(object sender, EventArgs e)
        {
           this.Close();
           FrmAccounts frmAccounts = new FrmAccounts(loginId);
           frmAccounts.Show();
        }

        private void serviceBtn_Click(object sender, EventArgs e)
        {
            this.Close();
            FrmServices frmServices = new FrmServices(loginId);
            frmServices.Show();
        }

        private void mngUserBtn_Click(object sender, EventArgs e)
        {
            this.Close();
            FrmUserManagement frmUserManagement = new FrmUserManagement(loginId);
            frmUserManagement.Show();
        }

        private void mngStaffBtn_Click(object sender, EventArgs e)
        {
            this.Close();
            FrmStaffManagement frmStaffManagement = new FrmStaffManagement(loginId);
            frmStaffManagement.Show();
        }

        private void reqBtn_Click(object sender, EventArgs e)
        {
            this.Close();
            FrmViewReq frmViewReq = new FrmViewReq(loginId);
            frmViewReq.Show();
        }

        private void viewAdminBtn_Click(object sender, EventArgs e)
        {
            FrmShowAdmins frmShowAdmins = new FrmShowAdmins();
            frmShowAdmins.Show();
        }

        private void changePassBtn_Click(object sender, EventArgs e)
        {
            FrmChangePass frmChangePass = new FrmChangePass(idt);
            frmChangePass.Show();
        }

        private void logoutBtn_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to logout?", "Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // 2. Show the Login form again
                // Note: Replace 'LoginForm' with the actual class name of your login window
                LoginForm login = new LoginForm();
                login.Show();

                // 3. Close the current dashboard
                this.Close();
            }

        }

        private void exitBtn_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to exit the application?", "Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                // 2. Completely terminate the application process
                Application.Exit();
            }

        }
    }
}
