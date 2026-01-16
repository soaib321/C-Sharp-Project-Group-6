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
    public partial class RoleSelectionForm : Form
    {
        private Form loginForm; // store LoginForm reference

        public RoleSelectionForm(Form login)
        {
            InitializeComponent();
            loginForm = login;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // User registration
            UserRegistrationForm userForm = new UserRegistrationForm(loginForm);
            userForm.Show();
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            StaffRegistrationForm staffForm = new StaffRegistrationForm(loginForm);
            staffForm.Show();
            this.Close();
        }
    }
}
