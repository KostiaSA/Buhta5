using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Buhta
{
    public class SQL
    {
        public const string ConnectionString = @"Data Source=dark\sql2005;Initial Catalog=Micronika_100114;User=sa;Password=sonyk";
        //public const string ConnectionString = "Data Source=ps-web;Initial Catalog=SUNERJA;User=sa1;Password=sonyk";
       

        public static SqlConnection GetOpenConnection(bool mars = false)
        {
            var cs = ConnectionString;
            if (mars)
            {
                SqlConnectionStringBuilder scsb = new SqlConnectionStringBuilder(cs);
                scsb.MultipleActiveResultSets = true;
                cs = scsb.ConnectionString;
            }
            var connection = new SqlConnection(cs);
            connection.Open();
            return connection;
        }
        public static SqlConnection GetClosedConnection()
        {
            return new SqlConnection(ConnectionString);
        }

    }
}
