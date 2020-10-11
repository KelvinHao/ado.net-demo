using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoDBUtil
{
    class DBUtils
    {
        public static SqlConnection GetConnection()
        {
            string dataSource = @"SE140675\SQLEXPRESS";
            string database = "DemoCsharp";
            string username = "sa";
            string password = "123";

            return DBSQLServerUtils.GetDBConnection(dataSource, database, username, password);
        }
    }
}
