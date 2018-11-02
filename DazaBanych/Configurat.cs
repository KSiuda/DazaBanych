using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;

namespace DazaBanych
{
    public partial class Configurat : Form
    {
        public Configurat()
        {
            InitializeComponent();
        }

        private void testButton_Click(object sender, EventArgs e)
        {
            string connectionString = string.Format($"Data Source={cboServer.Text};Initial Catalog={txtDB.Text};User ID={txtUser.Text};Password={txtPassword.Text}");
            try
            {
                SQLHelper helper = new SQLHelper(connectionString);
                if (helper.IsConnected)
                    MessageBox.Show("Connection successfull", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Configuration_Load(object sender, EventArgs e)
        {
           
            string con = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
            const string startText = "Data Source=";
            int start = con.LastIndexOf(startText) + startText.Length;
            int end = con.IndexOf("; Initial Catalog");
            int length = end - start;
            


            cboServer.Items.Add(ConfigurationManager.ConnectionStrings["con"].ConnectionString.Substring(start, length));
            cboServer.SelectedIndex = 0;

        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            string connectionString = string.Format($"Data Source={cboServer.Text};Initial Catalog={txtDB.Text};User ID={txtUser.Text};Password={txtPassword.Text}");
            try
            {
                SQLHelper helper = new SQLHelper(connectionString);
                if (helper.IsConnected)
                {
                    AppSetting setting = new AppSetting();
                    setting.SaveConnectionString("con", connectionString);
                    MessageBox.Show("Successfully saved", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
