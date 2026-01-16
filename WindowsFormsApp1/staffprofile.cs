using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class staffprofile : Form
    {
        int loginId;
        string connectionString = "Data Source=SOAIBS-LAPTOP\\SQLEXPRESS;Database=Project;Integrated Security=SSPI";

        public staffprofile(int loginId)
        {
            InitializeComponent();
            this.loginId = loginId;
        }

        private void staffprofile_Load(object sender, EventArgs e)
        {
            string query = "SELECT [S_Id], [Name], [Phone], [S_Type] FROM Staff WHERE loginId = @loginId";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@loginId", loginId);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    textBox1.Text = reader["S_Id"].ToString();
                    textBox2.Text = reader["Name"].ToString();
                    textBox3.Text = reader["Phone"].ToString();
                    textBox4.Text = reader["S_Type"].ToString();
                    textBox1.ReadOnly = true;
                    textBox4.ReadOnly = true;
                }
            }
        }

        // UPDATE
        private void button1_Click(object sender, EventArgs e)
        {
            string query = "UPDATE Staff SET Name = @Name, Phone = @Phone WHERE loginId = @loginId";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@loginId", loginId);
                command.Parameters.AddWithValue("@Name", textBox2.Text);
                command.Parameters.AddWithValue("@Phone", textBox3.Text);
                connection.Open();
                command.ExecuteNonQuery();
                MessageBox.Show("Profile updated.");
            }
        }

        // DELETE
        private void button3_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Delete profile?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                string query = "DELETE FROM Staff WHERE loginId = @loginId";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@loginId", loginId);
                    connection.Open();
                    command.ExecuteNonQuery();
                    Application.Restart(); // Re-log after deletion
                }
            }
        }

        // BACK
        private void button2_Click(object sender, EventArgs e)
        {
            staffdashboard dashboard = new staffdashboard(loginId);
            dashboard.Show();
            this.Close();
        }
    }
}