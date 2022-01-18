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
    public partial class EnrollForm : CatpureForm
    {
        public delegate void OnTemplateEventHandler(DPFP.Template template);
        public event OnTemplateEventHandler OnTemplate;
        private DPFP.Processing.Enrollment Enroller;
        //public byte[] serializedTemplate = null;
        // private finglist = new List<fmd>;

        protected override void Init()
        {            
            base.Init();
            base.Text = "Register";
            
            Enroller = new DPFP.Processing.Enrollment(); // Create an enrollment.
            UpdateStatus();
        }

        protected override void Process(DPFP.Sample Sample)
        {
            base.Process(Sample);

            // Process the sample and create a feature set for the enrollment purpose.
            DPFP.FeatureSet features = ExtractFeatures(Sample, DPFP.Processing.DataPurpose.Enrollment);

            // Check quality of the sample and add to enroller if it's good
            if (features != null) 
                try
                {
                    MakeReport("The fingerprint feature set was created.");
                    Enroller.AddFeatures(features);     // Add feature set to template.
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message+ ". Fingerprint features are indistinctive.", "Registration Failed",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
                finally
                {
                    UpdateStatus();
                    // Check if template has been created.
                    switch (Enroller.TemplateStatus)
                    {
                        case DPFP.Processing.Enrollment.Status.Ready:   // report success and stop capturing
                            OnTemplate(Enroller.Template);
                            //byte[] serializedTemplate = null;
                            MemoryStream stream = new MemoryStream();
                            Enroller.Template.Serialize(stream);
                            //Enroller.Template.Serialize(ref str_temp);
                            stream.Position = 0;
                            BinaryReader br = new BinaryReader(stream);
                            Byte[] bytes = br.ReadBytes((Int32)Enroller.Template.Bytes.Length);

                            if (stream != null)
                            {
                                //string result = System.Text.Encoding.UTF8.GetString(serializedTemplate);
                                //Console.Write(result);
                                
                                string Ename = GetVals.ename;
                                string Fname = GetVals.fname;
                                string Age = GetVals.age;
                                string address = GetVals.adr;
                                string Email = GetVals.Email;
                                string ContN = GetVals.contn;
                                string Sex = GetVals.sex;
                                string Eaddress = GetVals.eadd;
                                string Econt = GetVals.econt;
                                try
                                {
                                    if (Ename != "" && Fname != "" && Sex!="")
                                    {
                                        string valquery = "SELECT * FROM [EmployeesInfo] WHERE [Full_Name]=@Full_Name";
                                        using (SqlConnection con = new SqlConnection(@"Data Source=(LocalDb)\v11.0;AttachDbFilename=|DataDirectory|\Data.mdf;Integrated Security=True;User ID=try1;Password=password"))
                                        {
                                            using (SqlCommand cmd = new SqlCommand(valquery, con))
                                            {
                                                //cmd.Parameters.AddWithValue("@uname", uname);
                                                cmd.Parameters.AddWithValue("@Full_Name", Fname);
                                                SqlDataAdapter da = new SqlDataAdapter(cmd);
                                                DataTable dt = new DataTable();
                                                da.Fill(dt);

                                                if (dt.Rows.Count > 0)
                                                {
                                                    MessageBox.Show("Information found in the database. Please register a new employee.", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                                }
                                                else
                                                {
                                                    
                                                    string addquery = "INSERT INTO [EmployeesInfo] ([Full_Name],[Sex],[Age],[Address],[Email],[ContactN],[Ename],[Econtact],[Eaddress],[EmpFingerprint]) VALUES(@Full_Name,@Sex,@Age,@address,@Email,@ContactN,@Eneme,@ECont,@Eaddr,@serializedTemplate)";
                                                    SqlCommand cmdd = new SqlCommand(addquery, con);
                                                    
                                                    cmdd.Parameters.AddWithValue("@Full_Name", Fname);
                                                    cmdd.Parameters.AddWithValue("@Sex", Sex);
                                                    cmdd.Parameters.AddWithValue("@Age", Age);
                                                    cmdd.Parameters.AddWithValue("@address", address);
                                                    cmdd.Parameters.AddWithValue("@Email", Email);
                                                    cmdd.Parameters.AddWithValue("@ContactN", ContN);
                                                    cmdd.Parameters.AddWithValue("@Eneme", Ename);
                                                    cmdd.Parameters.AddWithValue("@ECont", Econt);
                                                    cmdd.Parameters.AddWithValue("Eaddr", Eaddress);
                                                    cmdd.Parameters.AddWithValue("@serializedTemplate ", SqlDbType.Binary).Value=bytes;

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
                                                        con.Close();
                                                        cmd.Parameters.Clear();
                                                        cmdd.Parameters.Clear();
                                                    }
                                                    switch (x)
                                                    {
                                                        case 1:
                                                            MessageBox.Show("Employee successfully added.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                                            SetPrompt("Closing...");
                                                            OnTemplate(Enroller.Template);
                                                            //serializedTemplate = null;
                                                            bytes = null;
                                                            features = null;
                                                            DialogResult = DialogResult.OK;
                                                            break;
                                                        case 0:
                                                            MessageBox.Show("Error");
                                                            features = null;
                                                            break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else{MessageBox.Show("Error!");}
                                }
                                catch (Exception ex)
                                { MessageBox.Show(ex.Message); }
                            }
                            Stop();
                            break;

                        case DPFP.Processing.Enrollment.Status.Failed:  // report failure and restart capturing
                            Enroller.Clear();
                            Stop();
                            UpdateStatus();
                            OnTemplate(null);
                            Start();
                            break;
                    }
                }
        }

        private void UpdateStatus()
        {
            // Show number of samples needed.
            SetStatus(String.Format("Fingerprint samples needed: {0}", Enroller.FeaturesNeeded));
        }

        public EnrollForm()
        {
            InitializeComponent();
        }
    }
}
