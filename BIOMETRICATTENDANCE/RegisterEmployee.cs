using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using System.Globalization;
using System.Text.RegularExpressions;

namespace BIOMETRICATTENDANCE
{

    public partial class RegisterEmployee : Form
    {
        private DPFP.Template Template;

        StringFormat strFormat = new StringFormat();//Used to format the grid rows.
        ArrayList arrColumnLefts = new ArrayList();//Used to save left coordinates of columns
        ArrayList arrColumnWidths = new ArrayList();//Used to save column widths
        int iCellHeight = 0; //Used to get/set the datagridview cell height
        int iTotalWidth = 0; //
        int iRow = 0;//Used as counter
        bool bFirstPage = false; //Used to check whether we are printing first page
        bool bNewPage = false;// Used to check whether we are printing a new page
        int iHeaderHeight = 0; //Used for the header height

        public RegisterEmployee()
        {
            InitializeComponent();
            datalods();
        }

        private void btnMenuSlide_Click(object sender, EventArgs e)
        {
            if (splitContainer1.SplitterDistance == 48)
            {
                splitContainer1.SplitterDistance = 250;
                panelSetting.Visible = true;
            }
            else 
            {
                splitContainer1.SplitterDistance = 48;
                panelSetting.Visible = false;
            }
                
        }

        private void btnCancelPass_Click(object sender, EventArgs e)
        {
            OldPasstxt.Text = NewPasstxt.Text = ConfirmPasstxt.Text = "";
        } // Cancel edit admin password

        private void btnSavePass_Click(object sender, EventArgs e)
        {
            if (NewPasstxt.Text == ConfirmPasstxt.Text && NewPasstxt.Text != OldPasstxt.Text && NewPasstxt.Text != "" && OldPasstxt.Text != "" & ConfirmPasstxt.Text != "")
            {
                SqlConnection con = new SqlConnection(@"Data Source=(LocalDb)\v11.0;AttachDbFilename=|DataDirectory|\Data.mdf;Integrated Security=True;User ID=try1;Password=password");
                //Data Source=(LocalDb)\v11.0;AttachDbFilename=|DataDirectory|\Databio.mdf;Integrated Security=True;User ID=try1;Password=password
                //Data Source=(LocalDb)\v11.0;AttachDbFilename="D:\2nd sem 2020-21\BIOMETRICATTENDANCE\BIOMETRICATTENDANCE\Databio.mdf";Integrated Security=True;User ID=try1
                con.Open();
                string query = "SELECT * FROM [Admin] WHERE [Password]= '" + OldPasstxt.Text.Trim() + "'";
                SqlDataAdapter sda = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string query1 = "UPDATE [Admin] SET [Password] = '" + ConfirmPasstxt.Text.Trim() + "' WHERE [Admin_ID] = '1'";

                        SqlCommand cmd = new SqlCommand(query1, con);
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }
                    MessageBox.Show("Password changed successfully!", " ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    OldPasstxt.Text = NewPasstxt.Text = ConfirmPasstxt.Text = "";
                    con.Close();
                    return;
                }
                else if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Incorrect password!","Error!",MessageBoxButtons.RetryCancel,MessageBoxIcon.Error);
                    OldPasstxt.Text = "";
                    con.Close();
                    return;
                }
                
            }
            else if(NewPasstxt.Text == OldPasstxt.Text)
            {
                MessageBox.Show("New password must differ from old password!", "Warning!", MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning);
                NewPasstxt.Text = ConfirmPasstxt.Text = "";
            }
            else if (NewPasstxt.Text=="" || OldPasstxt.Text=="" || ConfirmPasstxt.Text=="")
            {
                MessageBox.Show("Fill all fields!", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
                MessageBox.Show("Passwords does not match!", "Warning!", MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning);
                NewPasstxt.Text = ConfirmPasstxt.Text = "";

        } //Edit Admin Password

        private void btnAdd_Click(object sender, EventArgs e)
        {
            Regex reg = new Regex(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?");
            bool em = false;
            if (reg.IsMatch(tbEmail.Text))
            {
                 em = true;
            }
            else
            em = false;

            if (tbEName.Text != "" && tbFullName.Text != "" && em==true && tbAge.Text!="" && tbAddress.Text!="" && tbContac.Text!="" && tbEAddress.Text!="" & tbEContact.Text!=""  && (sexM.Checked || sexF.Checked) )
            {
                
                if (sexM.Checked) { GetVals.sex = "Male"; }
                else if (sexF.Checked) { GetVals.sex= "Female"; }
                GetVals.ename = tbEName.Text;
                GetVals.fname = tbFullName.Text;
                GetVals.age = tbAge.Text;
                GetVals.adr = tbAddress.Text;
                GetVals.Email = tbEmail.Text;
                GetVals.contn = tbContac.Text;
                GetVals.econt = tbEContact.Text;
                GetVals.eadd = tbEAddress.Text;

                
                EnrollForm fm = new EnrollForm();
                fm.OnTemplate += this.OnTemplate;
                fm.ShowDialog();
            }
            else if (tbEName.Text == "" || tbFullName.Text == "" || tbAge.Text=="" || tbAddress.Text=="" || tbContac.Text=="" || tbEContact.Text=="" || tbEAddress.Text=="" || (!sexM.Checked && !sexF.Checked) )
            {
                MessageBox.Show("Please fill all the required fields.", "Error!", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
            }
            else
                MessageBox.Show("Email '" + tbEmail.Text + "' does not match the expected format.", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        } //Add Employee

        private void bntUpdate_Click(object sender, EventArgs e)
        {
            string EmpID = tbEmpID.Text;
            string ename = tbEName.Text;
            string Fname = tbFullName.Text;
            string Sex = "";
            string Age = tbAge.Text;
            string Email = tbEmail.Text;
            string ContN = tbContac.Text;
            string address = tbAddress.Text;
            string Econt = tbEContact.Text;
            string Eaddress = tbEAddress.Text;
            
            if (sexM.Checked){Sex = "Male";}
            else if (sexF.Checked){Sex = "Female";}

            if (tbFullName.Text != "")
            {
                string valquery = "SELECT * FROM [EmployeesInfo] WHERE [Employee_ID] = @empidd";
                using (SqlConnection con = new SqlConnection(@"Data Source=(LocalDb)\v11.0;AttachDbFilename=|DataDirectory|\Data.mdf;Integrated Security=True;User ID=try1;Password=password"))
                {
                    using (SqlCommand cmd = new SqlCommand(valquery, con))
                    {
                        cmd.Parameters.AddWithValue("@empidd", EmpID);
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        if (dt.Rows.Count == 1)
                        {
                            string editquery = "UPDATE [EmployeesInfo] SET [Full_Name]=@Full_Name, [Sex]=@Sex, [Age]=@Age, [Address]=@address,  [Email]=@Email, [ContactN]=@ContactN, [Ename]=@Eneme, [Econtact]=@ECont, [Eaddress]=@Eaddr WHERE [Employee_ID]=@EmpID";
                            SqlCommand cmdd = new SqlCommand(editquery, con);
                            cmdd.Parameters.AddWithValue("@EmpID", EmpID);
                            cmdd.Parameters.AddWithValue("@Full_Name", Fname);
                            cmdd.Parameters.AddWithValue("@Sex", Sex);
                            cmdd.Parameters.AddWithValue("@Age", Age);
                            cmdd.Parameters.AddWithValue("@address", address);
                            cmdd.Parameters.AddWithValue("@Email", Email);
                            cmdd.Parameters.AddWithValue("@ContactN", ContN);
                            cmdd.Parameters.AddWithValue("@Eneme", ename);
                            cmdd.Parameters.AddWithValue("@ECont", Econt);
                            cmdd.Parameters.AddWithValue("Eaddr", Eaddress);
                            int x = 0;
                            try
                            {
                                con.Open();
                                x = cmdd.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message).ToString();
                            }
                            finally
                            {
                                cmdd.Parameters.Clear();
                                cmd.Parameters.Clear();
                                con.Close();
                                cmd.Dispose();
                                cmdd.Dispose();
                            }
                            switch (x)
                            {
                                case 1:
                                    MessageBox.Show("Employee successfully updated.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    tbEContact.Text = tbEAddress.Text = tbAddress.Text = TextBox4.Text =tbEName.Text=tbAge.Text = tbContac.Text = tbEmpID.Text = tbFullName.Text = tbEmail.Text = "";
                                    sexF.Checked = sexM.Checked = false;
                                    break;
                                case 0:
                                    MessageBox.Show("mali");
                                    break;
                            }

                           
                        }
                        else
                        {
                            MessageBox.Show("Information found in the database. Please register a new employee.", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Please fill all the required fields.", "Error!", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
            }
        } //Edit Employee



        private void TextBox4_TextChanged(object sender, EventArgs e)
        {

            string query = "SELECT * FROM [EmployeesInfo] WHERE [Employee_ID] LIKE '%" + TextBox4.Text + "%' OR [Full_Name] LIKE '%" + TextBox4.Text + "%'";
            using (SqlConnection con = new SqlConnection(@"Data Source=(LocalDb)\v11.0;AttachDbFilename=|DataDirectory|\Data.mdf;Integrated Security=True;User ID=try1;Password=password"))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {

                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        using (DataSet ds = new DataSet())
                        {
                            sda.Fill(ds);
                            
                            EmpDataLog.Refresh();
                            
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                tbEmpID.Text = ds.Tables[0].Rows[0][0].ToString();
                                tbFullName.Text = ds.Tables[0].Rows[0][1].ToString();
                                string sex = ds.Tables[0].Rows[0][2].ToString();
                                if (sex == "Male")
                                {
                                    sexM.Checked = true;
                                }
                                else
                                {
                                    sexF.Checked = true;
                                }
                                tbAge.Text = ds.Tables[0].Rows[0][3].ToString();
                                tbAddress.Text = ds.Tables[0].Rows[0][4].ToString();
                                tbEmail.Text = ds.Tables[0].Rows[0][5].ToString();
                                tbContac.Text = ds.Tables[0].Rows[0][6].ToString();
                                tbEName.Text = ds.Tables[0].Rows[0][8].ToString();
                                tbEContact.Text = ds.Tables[0].Rows[0][9].ToString();
                                tbEAddress.Text = ds.Tables[0].Rows[0][10].ToString();

                            }
                            else if (ds.Tables[0].Rows.Count==0 & TextBox4.Text!="")
                            {
                                MessageBox.Show("Employee not found!", "Result", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                tbEContact.Text = tbEAddress.Text = tbAddress.Text = TextBox4.Text = tbEName.Text = tbAge.Text = tbContac.Text = tbEmpID.Text = tbFullName.Text = tbEmail.Text = "";
                                sexF.Checked = sexM.Checked = false;
                            }
                            if (TextBox4.Text == "")
                            {
                                tbEContact.Text = tbEAddress.Text = tbAddress.Text = TextBox4.Text = tbEName.Text = tbAge.Text = tbContac.Text = tbEmpID.Text = tbFullName.Text = tbEmail.Text = "";
                                sexF.Checked = sexM.Checked = false;
                            }
                            
                            
                        }
                    }
                }
                //datalods();
            }      
        } //Search Employee

        private void RegisterEmployee_Load(object sender, EventArgs e)
        {
            EmpDataLog.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            EmpDataLog.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            EmpDataLog.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            EmpDataLog.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            EmpDataLog.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void OnTemplate(DPFP.Template template)
        {
            this.Invoke(new Function(delegate ()
            {
                Template = template;
                EnrollForm enrollForm = new EnrollForm();
                if (Template != null)
                {
                    if (DialogResult==DialogResult.OK)
                    datalods();
                    tbEContact.Text = tbEAddress.Text = tbAddress.Text = TextBox4.Text = tbEName.Text = tbAge.Text = tbContac.Text = tbEmpID.Text = tbFullName.Text = tbEmail.Text = "";
                    sexF.Checked = sexM.Checked = false;
                    template = null;
                    enrollForm.Close();
                }
                else
                {
                    MessageBox.Show("Please fill all the required fields.", "Error!", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                }
                
            }));
        }

        private void datalods()
        {
            SqlConnection con = new SqlConnection(@"Data Source=(LocalDb)\v11.0;AttachDbFilename=|DataDirectory|\Data.mdf;Integrated Security=True;User ID=try1;Password=password");
            con.Open();
            string query = "SELECT * FROM AttendanceDaily WHERE [Date]=@dt;";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@dt", SqlDbType.DateTime).Value = dtpStart.Value.Date;
            SqlDataAdapter sd = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sd.Fill(dt);
            EmpDataLog.DataSource = dt;
            cmd.Parameters.Clear();
            cmd.Dispose();
            con.Close();

            //if()
            //{
                con.Open();
                string rep = "SELECT [Full Name], COUNT([No.]) AS [Days Present] FROM AttendanceDaily WHERE MONTH([Date])=@m and YEAR([Date])=@y GROUP BY [Full Name]";
                SqlCommand rep1 = new SqlCommand(rep, con);
                rep1.Parameters.AddWithValue("@m", SqlDbType.DateTime).Value = DateTime.Now.Month;
                rep1.Parameters.AddWithValue("@y", SqlDbType.DateTime).Value = DateTime.Now.Year;
                SqlDataAdapter drep = new SqlDataAdapter(rep1);
                DataTable dtrep = new DataTable();
                drep.Fill(dtrep);
                ReportData.DataSource = dtrep;
                con.Close();
            //}
            
            EmpDataLog.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            EmpDataLog.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            EmpDataLog.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            EmpDataLog.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            EmpDataLog.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            ReportData.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            ReportData.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            foreach (DataGridViewColumn column in EmpDataLog.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            foreach (DataGridViewColumn cl in ReportData.Columns)
            {
                cl.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        } // show data

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection(@"Data Source=(LocalDb)\v11.0;AttachDbFilename=|DataDirectory|\Data.mdf;Integrated Security=True;User ID=try1;Password=password");
            con.Open();
            string query = "SELECT * FROM AttendanceDaily WHERE [Date]=@dt;";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@dt", SqlDbType.DateTime).Value = dtpStart.Value.Date;
            SqlDataAdapter sd = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sd.Fill(dt);
            EmpDataLog.DataSource = dt;
            cmd.Parameters.Clear();
            cmd.Dispose();
            con.Close();
        } // Check Attendance records by date

        private void EmpDataLog_SelectionChanged(object sender, EventArgs e)
        {
            EmpDataLog.ClearSelection();
        }

        private void ReportData_SelectionChanged(object sender, EventArgs e)
        {
            ReportData.ClearSelection();
        }

        private void fhalfBtn_Click(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection(@"Data Source=(LocalDb)\v11.0;AttachDbFilename=|DataDirectory|\Data.mdf;Integrated Security=True;User ID=try1;Password=password");
            con.Open();
            string rep = "SELECT [Full Name], COUNT([No.]) AS [Days Present] FROM AttendanceDaily WHERE DAY([Date]) BETWEEN 1 AND 15 AND MONTH([Date])=@m and YEAR([Date])=@y GROUP BY [Full Name]";
            SqlCommand rep1 = new SqlCommand(rep, con);
            rep1.Parameters.AddWithValue("@m", SqlDbType.DateTime).Value = DateTime.Now.Month;
            rep1.Parameters.AddWithValue("@y", SqlDbType.DateTime).Value = DateTime.Now.Year;
            SqlDataAdapter drep = new SqlDataAdapter(rep1);
            DataTable dtrep = new DataTable();
            drep.Fill(dtrep);
            ReportData.DataSource = dtrep;
            con.Close();
        } // Show Report First Half-month period 

        private void shalfBtn_Click(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection(@"Data Source=(LocalDb)\v11.0;AttachDbFilename=|DataDirectory|\Data.mdf;Integrated Security=True;User ID=try1;Password=password");
            con.Open();
            string rep = "SELECT [Full Name], COUNT([No.]) AS [Days Present] FROM AttendanceDaily WHERE DAY([Date]) BETWEEN 16 AND 31 AND MONTH([Date])=@m and YEAR([Date])=@y GROUP BY [Full Name]";
            SqlCommand rep1 = new SqlCommand(rep, con);
            rep1.Parameters.AddWithValue("@m", SqlDbType.DateTime).Value = DateTime.Now.Month;
            rep1.Parameters.AddWithValue("@y", SqlDbType.DateTime).Value = DateTime.Now.Year;
            SqlDataAdapter drep = new SqlDataAdapter(rep1);
            DataTable dtrep = new DataTable();
            drep.Fill(dtrep);
            ReportData.DataSource = dtrep;
            con.Close();
        } // Show Rerpot Second Half-month peroid

        private void printReportDocument_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            try
            {
                //Set the left margin
                int iLeftMargin = e.MarginBounds.Left;
                //Set the top margin
                int iTopMargin = e.MarginBounds.Top;
                //Whether more pages have to print or not
                bool bMorePagesToPrint = false;
                int iTmpWidth = 0;

                //For the first page to print set the cell width and header height
                if (bFirstPage)
                {
                    foreach (DataGridViewColumn GridCol in ReportData.Columns)
                    {
                        iTmpWidth = (int)(Math.Floor((double)((double)GridCol.Width /
                                       (double)iTotalWidth * (double)iTotalWidth *
                                       ((double)e.MarginBounds.Width / (double)iTotalWidth))));

                        iHeaderHeight = (int)(e.Graphics.MeasureString(GridCol.HeaderText,
                                    GridCol.InheritedStyle.Font, iTmpWidth).Height) + 11;

                        // Save width and height of headres
                        arrColumnLefts.Add(iLeftMargin);
                        arrColumnWidths.Add(iTmpWidth);
                        iLeftMargin += iTmpWidth;
                    }
                }
                //Loop till all the grid rows not get printed
                while (iRow <= ReportData.Rows.Count - 1)
                {
                    DataGridViewRow GridRow = ReportData.Rows[iRow];
                    //Set the cell height
                    iCellHeight = GridRow.Height + 5;
                    int iCount = 0;
                    //Check whether the current page settings allo more rows to print
                    if (iTopMargin + iCellHeight >= e.MarginBounds.Height + e.MarginBounds.Top)
                    {
                        bNewPage = true;
                        bFirstPage = false;
                        bMorePagesToPrint = true;
                        break;
                    }
                    else
                    {
                        if (bNewPage)
                        {
                            //Draw Header
                            e.Graphics.DrawString("Attendance Summary", new Font(ReportData.Font, FontStyle.Bold),
                                    Brushes.Black, e.MarginBounds.Left, e.MarginBounds.Top -
                                    e.Graphics.MeasureString("Attendance Summary", new Font(ReportData.Font,
                                    FontStyle.Bold), e.MarginBounds.Width).Height - 13);

                            String strDate = DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToShortTimeString();
                            //Draw Date
                            e.Graphics.DrawString(strDate, new Font(ReportData.Font, FontStyle.Bold),
                                    Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width -
                                    e.Graphics.MeasureString(strDate, new Font(ReportData.Font,
                                    FontStyle.Bold), e.MarginBounds.Width).Width), e.MarginBounds.Top -
                                    e.Graphics.MeasureString("Attendance Summary", new Font(new Font(ReportData.Font,
                                    FontStyle.Bold), FontStyle.Bold), e.MarginBounds.Width).Height - 13);

                            //Draw Columns                 
                            iTopMargin = e.MarginBounds.Top;
                            foreach (DataGridViewColumn GridCol in ReportData.Columns)
                            {
                                e.Graphics.FillRectangle(new SolidBrush(Color.LightGray),
                                    new Rectangle((int)arrColumnLefts[iCount], iTopMargin,
                                    (int)arrColumnWidths[iCount], iHeaderHeight));

                                e.Graphics.DrawRectangle(Pens.Black,
                                    new Rectangle((int)arrColumnLefts[iCount], iTopMargin,
                                    (int)arrColumnWidths[iCount], iHeaderHeight));

                                e.Graphics.DrawString(GridCol.HeaderText, GridCol.InheritedStyle.Font,
                                    new SolidBrush(GridCol.InheritedStyle.ForeColor),
                                    new RectangleF((int)arrColumnLefts[iCount], iTopMargin,
                                    (int)arrColumnWidths[iCount], iHeaderHeight), strFormat);
                                iCount++;
                            }
                            bNewPage = false;
                            iTopMargin += iHeaderHeight;
                        }
                        iCount = 0;
                        //Draw Columns Contents                
                        foreach (DataGridViewCell Cel in GridRow.Cells)
                        {
                            if (Cel.Value != null)
                            {
                                e.Graphics.DrawString(Cel.Value.ToString(), Cel.InheritedStyle.Font,
                                            new SolidBrush(Cel.InheritedStyle.ForeColor),
                                            new RectangleF((int)arrColumnLefts[iCount], (float)iTopMargin,
                                            (int)arrColumnWidths[iCount], (float)iCellHeight), strFormat);
                            }
                            //Drawing Cells Borders 
                            e.Graphics.DrawRectangle(Pens.Black, new Rectangle((int)arrColumnLefts[iCount],
                                    iTopMargin, (int)arrColumnWidths[iCount], iCellHeight));

                            iCount++;
                        }
                    }
                    iRow++;
                    iTopMargin += iCellHeight;
                }

                //If more lines exist, print another page.
                if (bMorePagesToPrint)
                    e.HasMorePages = true;
                else
                    e.HasMorePages = false;
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void printRep_Click(object sender, EventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            printDialog.Document = printReportDocument;
            printDialog.UseEXDialog = true;

            //Get the document
            if (DialogResult.OK == printDialog.ShowDialog())
            {
                printReportDocument.DocumentName = "Report";
                printReportDocument.Print();
            }
        } // Print

        private void printReportDocument_BeginPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            try
            {
                strFormat.Alignment = StringAlignment.Near;
                strFormat.LineAlignment = StringAlignment.Center;
                strFormat.Trimming = StringTrimming.EllipsisCharacter;

                arrColumnLefts.Clear();
                arrColumnWidths.Clear();
                iCellHeight = 0;
                iRow = 0;
                bFirstPage = true;
                bNewPage = true;

                // Calculating Total Widths
                iTotalWidth = 0;
                foreach (DataGridViewColumn dgvGridCol in ReportData.Columns)
                {
                    iTotalWidth += dgvGridCol.Width;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tbAge_TextChanged(object sender, EventArgs e)
        {
            int parsedValue;
            if (!int.TryParse(tbAge.Text, out parsedValue) && tbAge.Text!="")
            {
                MessageBox.Show("This is a number only field.", "Error", MessageBoxButtons.OK,MessageBoxIcon.Error);
                tbAge.Text = "";
                return;
            }
        }

        private void tbContac_TextChanged(object sender, EventArgs e)
        {
            Int64 parsedValue;
            if (!Int64.TryParse(tbContac.Text, out parsedValue) && tbContac.Text != "")
            {
                MessageBox.Show("This is a number only field.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbContac.Text = "";
                return;
            }
        }

        private void tbEContac_TextChanged(object sender, EventArgs e)
        {
            Int64 parsedValue;
            if (!Int64.TryParse(tbEContact.Text, out parsedValue) && tbEContact.Text != "")
            {
                MessageBox.Show("This is a number only field.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbEContact.Text = "";
                return;
            }
        }

    }
}
