using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FontAwesome.Sharp;
using System.Data.SqlClient;

namespace BIOMETRICATTENDANCE
{
    public partial class MainMenu : Form
    {
        private IconButton currentBtn;
        private Panel leftBorderBtn;
        public Form ccurrentChildForm;
        //private DPFP.Template Template;
        public MainMenu()
        {
            InitializeComponent();
            leftBorderBtn = new Panel();
            leftBorderBtn.Size = new Size(7, 49);
            panelMenu.Controls.Add(leftBorderBtn);
            this.Text = string.Empty;
            this.ControlBox = false;
            this.DoubleBuffered = true;
            //this.MaximizedBounds = Screen.FromHandle(this.Handle).WorkingArea;
        }

        private struct RGBColors
        {
            public static Color color1 = Color.FromArgb(0, 255, 255);
            public static Color color2 = Color.FromArgb(0, 255, 255);
        }

        private void ActivateButton(object senderBtn, Color color)
        {
            if (senderBtn != null)
            {
                DisableButton();
                currentBtn = (IconButton)senderBtn;
                currentBtn.BackColor = Color.FromArgb(33,33,33);
                currentBtn.ForeColor = color;
                currentBtn.TextAlign = ContentAlignment.MiddleCenter;
                currentBtn.IconColor = color;
                currentBtn.TextImageRelation = TextImageRelation.TextBeforeImage;
                currentBtn.ImageAlign = ContentAlignment.MiddleRight;

                leftBorderBtn.BackColor = color;
                leftBorderBtn.Location = new Point(0, currentBtn.Location.Y);
                leftBorderBtn.Visible = true;
                leftBorderBtn.BringToFront();

                currentChildForm.IconChar = currentBtn.IconChar;
                currentChildForm.IconColor = color;
                
            }
        }

        private void DisableButton()
        {
            if (currentBtn != null )
            {
                currentBtn.BackColor = Color.FromArgb(0, 166, 255);
                currentBtn.ForeColor = Color.GhostWhite;
                currentBtn.TextAlign = ContentAlignment.MiddleLeft;
                currentBtn.IconColor = Color.GhostWhite;
                currentBtn.TextImageRelation = TextImageRelation.ImageBeforeText;
                currentBtn.ImageAlign = ContentAlignment.MiddleLeft;
            }
        }

        private void OpenChildForm(Form childForm)
        {
            if (ccurrentChildForm != null)
            {
                ccurrentChildForm.Close();
            }
            ccurrentChildForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Size = panelDesktop.Size;
            childForm.Dock = DockStyle.Fill;
            panelDesktop.Controls.Add(childForm);
            panelDesktop.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            Reset();
            if (ccurrentChildForm != null)
            {
                ccurrentChildForm.Close();
            }
            PassForm adminpass = new PassForm()
            {
                StartPosition = FormStartPosition.CenterScreen
            };
            DialogResult result = adminpass.ShowDialog();
            if (result == DialogResult.OK)
            {
                MessageBox.Show("Welcome admin!","Admin",MessageBoxButtons.OK, MessageBoxIcon.Information);
                ActivateButton(sender, RGBColors.color1);
                lblCurrentChildForm.Text = "Admin";
                lblCurrentChildForm.ForeColor = RGBColors.color1;
                OpenChildForm(new RegisterEmployee());
            }
            else if(result==DialogResult.Retry)
            {
                MessageBox.Show("Incorrect password!", "Error!",MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
                adminpass.Close();
        } // Admin Login

        private void iconButton2_Click(object sender, EventArgs e)
        {
            Reset();
            if (ccurrentChildForm != null)
            {
                ccurrentChildForm.Close();
            }
            VerifyForm verify = new VerifyForm()
            {
                StartPosition = FormStartPosition.CenterScreen
            };
            DialogResult result = verify.ShowDialog();
            if (result == DialogResult.OK)
            {
                MessageBox.Show("Welcome employee "+ GetVals.user +".", "User", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ActivateButton(sender, RGBColors.color2);
                lblCurrentChildForm.Text = "Attendance";
                lblCurrentChildForm.ForeColor = RGBColors.color2;
                OpenChildForm(new AttendanceForm());
            }
            else if (result == DialogResult.Retry)
            {
                MessageBox.Show("Employee not found!", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
                verify.Close();
        } // Employee attendance login

        private void Reset()
        {
            DisableButton();
            leftBorderBtn.Visible = false;
            currentChildForm.IconChar = IconChar.Home;
            currentChildForm.IconColor = Color.GhostWhite;
            lblCurrentChildForm.Text = "Home";
            lblCurrentChildForm.ForeColor = Color.GhostWhite;
        }

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);

        private void panelTitleBar_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void btnExt_Click(object sender, EventArgs e)
        {
            Application.Exit();
        } // Exit application

        private void btnMinMax_Click(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
                WindowState = FormWindowState.Maximized;
            else
                WindowState = FormWindowState.Normal;
        } // Restore/Maximze app

        private void btnMin_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        } // Minimize app

        private void tmDatTim_Tick(object sender, EventArgs e)
        {
            lblDate.Text = DateTime.Now.ToLongDateString();
            lblTime.Text = DateTime.Now.ToString("hh:mm:ssss tt");
        } // Display Date & Time

        private void label1_Click(object sender, EventArgs e)
        {
            Reset();
            if (ccurrentChildForm != null)
                ccurrentChildForm.Close();
        } // Home button

        private void label2_Click(object sender, EventArgs e)
        {
            Reset();
            if (ccurrentChildForm != null)
                ccurrentChildForm.Close();
        } // Home Button

        //private void MainMenu_Resize(object sender, EventArgs e)
        //{
        //    if (WindowState == FormWindowState.Maximized)
        //        FormBorderStyle = FormBorderStyle.None;
        //    else
        //        FormBorderStyle = FormBorderStyle.Sizable;
        //}
    }
}
