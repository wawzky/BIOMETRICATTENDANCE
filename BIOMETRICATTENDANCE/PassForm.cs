using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace BIOMETRICATTENDANCE
{
    public partial class PassForm : Form
    {
        public PassForm()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection(@"Data Source=(LocalDb)\v11.0;AttachDbFilename=|DataDirectory|\Data.mdf;Integrated Security=True;User ID=try1;Password=password");
            //Data Source=(LocalDb)\v11.0;AttachDbFilename="D:\2nd sem 2020-21\BIOMETRICATTENDANCE\BIOMETRICATTENDANCE\Databio.mdf";Integrated Security=True;User ID=try1
            string query = "SELECT * FROM [Admin] WHERE [Password]= '" + Passbox.Text.Trim() + "'";
            SqlDataAdapter sda = new SqlDataAdapter(query, con);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            if (dt.Rows.Count == 1)
            {
                DialogResult = DialogResult.OK;
            }
            else
                DialogResult = DialogResult.Retry;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
