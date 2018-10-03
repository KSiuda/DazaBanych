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

namespace DazaBanych
{
    public partial class BizContacts : Form
    {
        string connectString = @"Data Source=den1.mssql3.gear.host;Initial Catalog=addressbook1;User ID=addressbook1;Password=Pp39QUmWc!!h;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        SqlDataAdapter dataAdapter; //object to connect program with DB
        DataTable table; // variable program table



        public BizContacts()
        {
            InitializeComponent();
        }

        private void BizContacts_Load(object sender, EventArgs e)
        {
            cboSearch.SelectedIndex = 0; //first item in combobox is selected when the form loads
            dataGridView1.DataSource = bindingSource1; //sets the source of the data to be displ;ayed in the grid view object

            GetData("Select * from BizContacts;");
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
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message); //show a message about the exception
            }


        }

        private void btnAdd_Click(object sender, EventArgs e)
        {

            SqlCommand command;
            string insert = @"INSERT INTO BizContacts 
                                                    (Date_Added, Company, Website, Title, First_Name, Last_Name, Address, City, State, Postal_Code, Email, Mobile, Notes)
                              VALUES 
                                                    (@Date_Added, @Company, @Website, @Title, @First_Name, @Last_Name, @Address, @City, @State, @Postal_Code, @Email, @Mobile, @Notes);";

            using (SqlConnection conn = new SqlConnection(connectString)) //allows cleaning up low level resources
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
                    command.ExecuteNonQuery(); //push stuff into the table
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
            GetData("SELECT * FROM BizContacts;"); //gets all data - including about lately inserted bizcontact
            dataGridView1.Update(); //updates datagridview with freshly selected information

        }
    }
}