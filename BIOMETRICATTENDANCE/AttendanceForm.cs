using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

namespace BIOMETRICATTENDANCE
{
    public partial class AttendanceForm : Form
    {
        //private DPFP.Template Template;
        public AttendanceForm()
        {
            InitializeComponent();
            System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath();
            gp.AddEllipse(0, 0,pbProf.Height,pbProf.Width);
            Region rg = new Region(gp);
            pbProf.Region = rg;
            panelSetting.Paint += new System.Windows.Forms.PaintEventHandler(groupBox2_Paint);
            datagridload();
        }

        private void datagridload()
        {
            SqlConnection con = new SqlConnection(@"Data Source=(LocalDb)\v11.0;AttachDbFilename=|DataDirectory|\Data.mdf;Integrated Security=True;User ID=try1;Password=password");
            con.Open();
            string query = "SELECT * FROM AttendanceDaily WHERE [Date]=@dt;";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@dt", SqlDbType.DateTime).Value = DateTime.Now.Date;
            SqlDataAdapter sd = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sd.Fill(dt);
            EmpDataLog.DataSource = dt;
            cmd.Dispose();
            con.Close();

            // Now that DataGridView has calculated it's Widths; we can now store each column Width values.
            //for (int i = 0; i <= EmpDataLog.Columns.Count - 1; i++)
            //{
            //    // Store Auto Sized Widths:
            //    int colw = EmpDataLog.Columns[i].Width;
            //    // Remove AutoSizing:
            //    EmpDataLog.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //    // Set Width to calculated AutoSize value:
            //    EmpDataLog.Columns[i].Width = colw;
            //}
            
            foreach (DataGridViewColumn column in EmpDataLog.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            EmpDataLog.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            EmpDataLog.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            EmpDataLog.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            EmpDataLog.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            EmpDataLog.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        } // Show data

        private void tmDatTim_Tick(object sender, EventArgs e)
        {
            lblDate.Text = DateTime.Now.ToLongDateString();
            lblTime.Text = DateTime.Now.ToString("hh:mm:ssss tt");
        }//Display Time and Date

        private void AddEllipseExample(PaintEventArgs e)
        {
            // Create a path and add an ellipse.
            Rectangle myEllipse = new Rectangle(pbProf.Location.X, pbProf.Location.Y, 226, 226);
            GraphicsPath myPath = new GraphicsPath();
            myPath.AddEllipse(myEllipse);

            // Draw the path to the screen.
            Pen myPen = new Pen(Color.FromArgb(50, 25, 170), 13);
            e.Graphics.DrawPath(myPen, myPath);
        } 

        private void AttendanceForm_Load(object sender, EventArgs e)
        {
            lblUsername.Text = GetVals.user;
        }

        private void EmpDataLog_SelectionChanged(object sender, EventArgs e)
        {
            EmpDataLog.ClearSelection();
        }

        private void groupBox2_Paint(object sender, PaintEventArgs e)
        {
            AddEllipseExample(e);
        }
    }
}
