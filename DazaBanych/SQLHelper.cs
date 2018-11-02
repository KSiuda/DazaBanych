using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;

namespace DazaBanych
{
    public class SQLHelper
    {
        SqlConnection con;

        public SQLHelper(string connectionString)
        {
            con = new SqlConnection(connectionString);
        }

        public bool IsConnected
        {
            get
            {
                if (con.State == System.Data.ConnectionState.Closed)
                    con.Open();
                return true;
            }
        }
    }
}
