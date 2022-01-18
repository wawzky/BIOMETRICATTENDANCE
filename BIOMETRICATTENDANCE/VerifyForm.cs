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
using System.IO;

namespace BIOMETRICATTENDANCE
{
    public partial class VerifyForm : CatpureForm
    {

        //private DPFP.Template tmpObj;
        private DPFP.Verification.Verification Verificator;


        public void Verify(DPFP.Template template)
        {
            //tmpObj = template;
            ShowDialog();
        }

        protected override void Init()
        {
            base.Init();
            base.Text = "Verify";
            Verificator = new DPFP.Verification.Verification();     // Create a fingerprint template verificator
            UpdateStatus(0);
        }

        private void UpdateStatus(int FAR)
        {
            // Show "False accept rate" value
            SetStatus(String.Format("Fingerprint Matched = {0} %", FAR));
        }

        protected override void Process(DPFP.Sample Sample)
        {

            // Compare the feature set with our template
            try
            {
                using (SqlConnection con = new SqlConnection(@"Data Source=(LocalDb)\v11.0;AttachDbFilename=|DataDirectory|\Data.mdf;Integrated Security=True;User ID=try1;Password=password"))
                //Data Source=(LocalDb)\v11.0;AttachDbFilename=|DataDirectory|\Databio.mdf;Integrated Security=True;User ID=try1;Password=password -- tama
                {
                    con.Open();
                    try
                    {
                        //AttendanceForm frm = new AttendanceForm();
                        SqlCommand cmd = new SqlCommand("SELECT * FROM [EmployeesInfo] WHERE [EmpFingerprint] IS NOT NULL;", con);
                        SqlDataAdapter dr = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        dr.Fill(dt);
                        foreach (DataRow d in dt.Rows)
                        {
                            string fname = (string)d["Full_Name"];
                            byte[] fpbytes = (byte[])d["EmpFingerprint"];

                            MemoryStream stream = new MemoryStream(fpbytes);
                            DPFP.Template tmpObj = new DPFP.Template();
                            tmpObj.DeSerialize(stream);

                            base.Process(Sample);
                            // Process the sample and create a feature set for the enrollment purpose.
                            DPFP.FeatureSet features = ExtractFeatures(Sample, DPFP.Processing.DataPurpose.Verification);

                            // Check quality of the sample and start verification if it's good
                            // TODO: move to a separate task
                            if (features != null)
                            {
                                DPFP.Verification.Verification.Result result = new DPFP.Verification.Verification.Result();
                                // Compare the feature set with our template     
                                Verificator.Verify(features, tmpObj, ref result);
                                UpdateStatus(100 - result.FARAchieved);
                                cmd.Dispose();
                                con.Close();
                                if (result.Verified)
                                {
                                    DateTime wen = DateTime.Now;
                                    DateTime dat = DateTime.Now.Date;
                                    string inn = wen.ToString("hh:mm:tt");
                                    string outt = wen.ToString("hh:mm:tt");
                                    GetVals.user =fname;

                                    con.Open();
                                    SqlCommand ver = new SqlCommand("SELECT * FROM [AttendanceDaily] WHERE [Full Name]='" + fname + "' AND [Date]=@wenn;", con);
                                    ver.Parameters.AddWithValue("@wenn", SqlDbType.Date).Value = wen.Date;
                                    ver.ExecuteNonQuery();
                                    SqlDataAdapter drv = new SqlDataAdapter(ver);
                                    DataTable dtv = new DataTable();
                                    drv.Fill(dtv);
                                    ver.Parameters.Clear();
                                    ver.Dispose();
                                    con.Close();
                                    if (dtv.Rows.Count == 1)
                                    {
                                        con.Open();
                                        SqlCommand verd = new SqlCommand("SELECT * FROM [AttendanceDaily] WHERE [Full Name]='" + fname + "' AND [Time Out] IS NULL AND [Time In] IS NOT NULL;", con);
                                        SqlDataAdapter drvd = new SqlDataAdapter(verd);
                                        DataTable dtvd = new DataTable();
                                        drvd.Fill(dtvd);
                                        verd.Dispose();
                                        con.Close();
                                        if (dtvd.Rows.Count > 0)
                                        {
                                            con.Open();
                                            SqlCommand cmdupout = new SqlCommand("UPDATE [AttendanceDaily] SET [Time Out]=@tout WHERE [Full Name]='" + fname + "';", con);
                                            cmdupout.Parameters.AddWithValue("@tout", outt);
                                            cmdupout.ExecuteNonQuery();
                                            cmdupout.Dispose();
                                            cmdupout.Parameters.Clear();
                                            con.Close();
                                        }
                                        else
                                            SetPrompt("Closing...");
                                        MessageBox.Show("You have already submitted attendance for today!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    }
                                    else if (dtv.Rows.Count == 0)
                                    {
                                        con.Open();
                                        SqlCommand cmdd = new SqlCommand("INSERT INTO [AttendanceDaily] ([Full Name],[Date],[Time In]) VALUES(@fname,@wen,@tinn);", con);
                                        cmdd.Parameters.AddWithValue("@fname", fname);
                                        cmdd.Parameters.AddWithValue("@wen", SqlDbType.Date).Value = wen.Date;
                                        cmdd.Parameters.AddWithValue("@tinn", inn);
                                        cmdd.ExecuteNonQuery();
                                        cmdd.Parameters.Clear();
                                        cmdd.Dispose();
                                        con.Close();
                                    }
                                    MakeReport("The fingerprint was VERIFIED.");
                                    SetPrompt("Closing...");
                                    Stop();
                                    this.DialogResult = DialogResult.OK;
                                    break;
                                }
                                else if (result.Verified == false)
                                {
                                    //MakeReport("The fingerprint was NOT VERIFIED.");
                                    SetPrompt("Fingerprint not Verified. Please try again.");
                                    UpdateStatus(0);
                                    //this.DialogResult = DialogResult.Retry;
                                    con.Close();
                                }

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    finally
                    {
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        public VerifyForm()
        {
            InitializeComponent();
        }
    }
}
