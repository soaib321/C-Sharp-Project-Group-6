using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class cleanreq : Form
    {
        public cleanreq()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        int userId;
        int serviceId;
        

      

    
        public cleanreq(int userId, int serviceId)
        {
            InitializeComponent();
           
            this.userId = userId;
            this.serviceId = serviceId;
            
        }
        //private void textBox1_TextChanged(object sender, EventArgs e)
        //{ textBox1.Text = "Pending"; }


        private void button1_Click(object sender, EventArgs e)
        {
            string connectionString = "data source=SOAIBS-LAPTOP\\SQLEXPRESS; database=Project; integrated security=SSPI";
            string ctype = comboBox1.SelectedItem.ToString();
            DateTime reqd = dateTimePicker1.Value;
            string status = textBox1.Text;

            if (string.IsNullOrWhiteSpace(ctype)
 )
            {
                MessageBox.Show("All fields must be filled out.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string query = "INSERT INTO [C_Req] (C_Type,Status,C_Date,UserId) VALUES (@C_Type, @Status, @C_Date,@UserId)";
            
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@C_type", ctype);

                    command.Parameters.AddWithValue("@Status", status);
                    command.Parameters.AddWithValue("@C_Date", reqd);
                    command.Parameters.AddWithValue("@UserId", this.userId);
                    //command.Parameters.AddWithValue("@ServiceID", this.serviceId);
                    

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Request created successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Hide();
                        Userdashboard f1 = new Userdashboard();
                        f1.Show();
                    }
                    else
                    {
                        MessageBox.Show("Failed to create the Request. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }






                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
          
            comboBox1.SelectedIndex = -1;

            dateTimePicker1.Value = DateTime.Now;

            textBox1.Text = "Pending";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Userdashboard dashboard = new Userdashboard();
            dashboard.Show();

            this.Close();
        }

        private void cleanreq_Load(object sender, EventArgs e)
        {
            textBox1.Text = "Pending";

            // Prevent the user from typing in it
            textBox1.ReadOnly = true;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }
    }
}
