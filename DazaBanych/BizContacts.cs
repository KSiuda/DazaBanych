using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.IO;
using System.Configuration;


namespace DazaBanych
{
    public partial class BizContacts : Form
    {
        string connectString = ConfigurationManager.ConnectionStrings["con"].ConnectionString;


        SqlDataAdapter dataAdapter; //object to connect program with DB
        DataTable table; // variable program table
        SqlCommandBuilder commandBuilder;
        SqlConnection conn;
        string selectionStatement = "Select * from BizContacts;";



        public BizContacts()
        {
            InitializeComponent();
        }

        private void BizContacts_Load(object sender, EventArgs e)
        {
            cboSearch.SelectedIndex = 0; //first item in combobox is selected when the form loads
            dataGridView1.DataSource = bindingSource1; //sets the source of the data to be displ;ayed in the grid view object

            GetData(selectionStatement);
        }

        private void GetData(string selectCommand)
        {

            try
            {
                dataAdapter = new SqlDataAdapter(selectCommand, connectString); //pass in the select command, and connect to DB using the connection string
                table = new DataTable();
                table.Locale = System.Globalization.CultureInfo.InvariantCulture;
                dataAdapter.Fill(table); // fill the datatable
                bindingSource1.DataSource = table; //set the datasource on the binding source to the table
                dataGridView1.Columns[0].ReadOnly = true;
            }
            catch(SqlException ex)
            {
                MessageBox.Show(ex.Message); //show a message about the exception
            }


        }

        private void btnAdd_Click(object sender, EventArgs e)
        {

            SqlCommand command;
            string insert = @"INSERT INTO BizContacts 
                                                    (Date_Added, Company, Website, Title, First_Name, Last_Name, Address, City, State, Postal_Code, Email, Mobile, Notes, Image)
                              VALUES 
                                                    (@Date_Added, @Company, @Website, @Title, @First_Name, @Last_Name, @Address, @City, @State, @Postal_Code, @Email, @Mobile, @Notes, @Image);";

            using (conn = new SqlConnection(connectString)) //allows cleaning up low level resources
            {

                try
                {
                    conn.Open(); //open connection
                    command = new SqlCommand(insert, conn);
                    command.Parameters.AddWithValue(@"Date_Added", dateTimePicker1.Value.Date); //read value from form and save to the table
                    command.Parameters.AddWithValue(@"Company", txtCompany.Text);
                    command.Parameters.AddWithValue(@"Website", txtWebsite.Text);
                    command.Parameters.AddWithValue(@"Title", txtTitle.Text);
                    command.Parameters.AddWithValue(@"First_Name", txtFName.Text);
                    command.Parameters.AddWithValue(@"Last_Name", txtLName.Text);
                    command.Parameters.AddWithValue(@"Address", txtAddress.Text);
                    command.Parameters.AddWithValue(@"City", txtCity.Text);
                    command.Parameters.AddWithValue(@"State", txtState.Text);
                    command.Parameters.AddWithValue(@"Postal_Code", txtZip.Text);
                    command.Parameters.AddWithValue(@"Email", txtEmail.Text);
                    command.Parameters.AddWithValue(@"Mobile", txtMobile.Text);
                    command.Parameters.AddWithValue(@"Notes", txtNotes.Text);
                    command.Parameters.AddWithValue(@"Image", File.ReadAllBytes(dlgOpenImage.FileName)); //convert image to bytes for saving    
                    command.ExecuteNonQuery(); //push stuff into the table
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                }
            GetData(selectionStatement); //gets all data - including about lately inserted bizcontact
            dataGridView1.Update(); //updates datagridview with freshly selected information
                                    
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            commandBuilder = new SqlCommandBuilder(dataAdapter);
            dataAdapter.UpdateCommand = commandBuilder.GetDeleteCommand();
            try
            {
                bindingSource1.EndEdit();  //software table in memory update
                dataAdapter.Update(table); //db update
                MessageBox.Show("Update Successfull - DB updated");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DataGridViewRow row = dataGridView1.CurrentCell.OwningRow; //grab a reference to the current row
            string value = row.Cells["ID"].Value.ToString(); // grab the value from the id field
            string fname = row.Cells["First_Name"].Value.ToString(); // grab the value from the id field
            string lname = row.Cells["Last_Name"].Value.ToString(); // grab the value from the id field
            DialogResult result = MessageBox.Show("Do you want to delete " + fname + " " + lname + ", record" + value +"?","Message", MessageBoxButtons.YesNo,MessageBoxIcon.Question);

            string deleteStatement = @"DELETE FROM BizContacts WHERE ID = '"+value+"';"; 

            if (result==DialogResult.Yes)
            {
                using (conn = new SqlConnection(connectString))
                {
                    try
                    {
                        conn.Open();
                        SqlCommand comm = new SqlCommand(deleteStatement,conn);
                        comm.ExecuteNonQuery();
                        GetData(selectionStatement);
                        dataGridView1.Update();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            switch(cboSearch.SelectedItem.ToString())
            {
                case "First Name":
                    GetData("SELECT * FROM BizContacts WHERE LOWER(FIRST_NAME) LIKE'%" + txtSearch.Text.ToLower() + "%'");
                    break;
                case "Last Name":
                    GetData("SELECT * FROM BizContacts WHERE LOWER(LAST_NAME) LIKE'%" + txtSearch.Text.ToLower() + "%'");
                    break;
                case "Company":
                    GetData("SELECT * FROM BizContacts WHERE LOWER(COMPANY) LIKE'%" + txtSearch.Text.ToLower() + "%'");
                    break;
                case "":
                    GetData("SELECT * FROM BizContacts;");
                    break;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dlgOpenImage.ShowDialog();
            pictureBox1.Load(dlgOpenImage.FileName); 
        }
    }
}
