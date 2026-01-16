using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Userdashboard : Form
    {
        public Userdashboard()
        {
            InitializeComponent();
        }
        int userId;

        public Userdashboard(int userId)
        {
            InitializeComponent();
            this.userId =userId;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            int serviceId = 4;
            wastereq f1 = new wastereq(userId, serviceId);
            
            f1.Show();
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            int serviceId = 3;
            pesreq f2 = new pesreq(userId, serviceId);
            f2.Show();

            
        }

        private void button3_Click(object sender, EventArgs e)
        {  this.Hide();
            int serviceId = 1;
            
            cleanreq f3 =new cleanreq(userId, serviceId);
            f3.Show();

        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Hide();
            allreq f4 = new allreq(this.userId);
            f4.Show();

        }

        private void Userdashboard_Load(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to logout?", "Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
               
                LoginForm login = new LoginForm();
                login.Show();

                this.Close();
            }
        }
    }
}
