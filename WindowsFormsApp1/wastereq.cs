using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Collections.Specialized.BitVector32;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WindowsFormsApp1
{
    public partial class wastereq : Form
    {
        public wastereq()
        {
            InitializeComponent();
        }
        int userId;
        int serviceId;




         
        public wastereq(int userId, int serviceId)
        {
            InitializeComponent();

            this.userId = userId;
            this.serviceId = serviceId;

        }



        //private void textBox1_TextChanged(object sender, EventArgs e)
        //{
        //    textBox1.Text = "Pending";
        //}

        private void button1_Click(object sender, EventArgs e)
        {
            string connectionString = "data source=SOAIBS-LAPTOP\\SQLEXPRESS; database=Project; integrated security=SSPI";
            string wtype = comboBox1.SelectedItem.ToString();
            DateTime cod = dateTimePicker1.Value;          
            int apweight = (int)numericUpDown1.Value;
            string status = "Pending";
           
            

            if (string.IsNullOrWhiteSpace(wtype) 
  )
            {
                MessageBox.Show("All fields must be filled out.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string query = "INSERT INTO [W_Req] (W_type, Quantity ,Status,Req_Date,UserId,[ServiceID]) VALUES (@W_type, @Quantity, @Status, @Req_Date,@UserId,@ServiceID)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@W_type", wtype);
                    command.Parameters.AddWithValue("@Quantity", apweight);
                    command.Parameters.AddWithValue("@Status", status);
                    command.Parameters.AddWithValue("@Req_Date", cod);
                    command.Parameters.AddWithValue("@UserId", userId);
                   
                    command.Parameters.AddWithValue("@ServiceID", serviceId);
                    
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

            numericUpDown1.Value = numericUpDown1.Minimum;

            dateTimePicker1.Value = DateTime.Now;

            textBox1.Text = "Pending";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Userdashboard dashboard = new Userdashboard();
            dashboard.Show();

            this.Close();
        }

        private void wastereq_Load(object sender, EventArgs e)
        {
            textBox1.Text = "Pending";

    // Prevent the user from typing in it
    textBox1.ReadOnly = true;

        }
    }
}