using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WindowsFormsApp1
{
    public partial class pesreq : Form
    {
        public pesreq()
        {
            InitializeComponent();
        }
        int userId;
        int serviceId;





        public pesreq(int userId, int serviceId)
        {
            InitializeComponent();

            this.userId = userId;
            this.serviceId = serviceId;

        }

        //private void textBox2_TextChanged(object sender, EventArgs e)
        //{
        //    textBox2.Text = "Pending";
        //}

        private void button1_Click(object sender, EventArgs e)
        {
            string connectionString = "data source=SOAIBS-LAPTOP\\SQLEXPRESS; database=Project; integrated security=SSPI";
            string ptype = comboBox1.SelectedItem.ToString();
            string ttype = comboBox2.SelectedItem.ToString();
            string carea = comboBox3.SelectedItem.ToString();
            DateTime cod = dateTimePicker1.Value;

            string status = textBox2.Text;

            if (string.IsNullOrWhiteSpace(ptype) || string.IsNullOrWhiteSpace(ttype) || string.IsNullOrWhiteSpace(carea)
 )
            {
                MessageBox.Show("All fields must be filled out.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string query = "INSERT INTO [Pes_Req] (PestType, TreatmentType,[CoverageArea] ,Status,[RequestDate],UserId,ServiceID ) VALUES (@PestType, @TreatmentType,@CoverageArea, @Status, @RequestDate,@UserId,@ServiceID)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PestType", ptype);
                    command.Parameters.AddWithValue("@TreatmentType", ttype);
                    command.Parameters.AddWithValue("@CoverageArea", carea);
                    command.Parameters.AddWithValue("@Status", status);
                    command.Parameters.AddWithValue("@RequestDate", cod);
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
            comboBox2.SelectedIndex = -1;
            comboBox3.SelectedIndex = -1;

            dateTimePicker1.Value = DateTime.Now;

            textBox2.Text = "Pending";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Userdashboard dashboard = new Userdashboard();
            dashboard.Show();

            this.Close();
        }

        private void pesreq_Load(object sender, EventArgs e)
        {
            textBox2.Text = "Pending";

            // Prevent the user from typing in it
            textBox2.ReadOnly = true;
        }
    }
}
