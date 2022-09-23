using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankomatM
{
    internal class DBUtils
    {
        public static MySqlConnection GetDBConnection()
        {
            string host = "SQL_SERVER_ip_address";
            int port = SQL_SERVER_port;
            string database = "DB_name";
            string username = "DB_user";
            string password = "DP_user_password";

            return DBMySQLUtils.GetDBConnection(host, port, database, username, password);
        }
    }
}
