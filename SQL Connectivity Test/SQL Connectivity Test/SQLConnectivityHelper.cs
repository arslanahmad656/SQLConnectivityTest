using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQL_Connectivity_Test
{
    static class SQLConnectivityHelper
    {
        public static SqlConnection GetSQLConnection(string server, string database, bool windowsAuth, string username, string password)
        {
            if (string.IsNullOrWhiteSpace(server))
            {
                throw new ArgumentNullException(nameof(server));
            }

            if (!windowsAuth)
            {
                if (string.IsNullOrWhiteSpace(username))
                {
                    throw new ArgumentNullException(nameof(username));
                }

                if (string.IsNullOrWhiteSpace(password))
                {
                    throw new ArgumentNullException(nameof(password));
                }
            }

            var connectionStringBuilder = new SqlConnectionStringBuilder();
            connectionStringBuilder.DataSource = server;
            if (!string.IsNullOrWhiteSpace(database))
            {
                connectionStringBuilder.InitialCatalog = database;
            }

            if (windowsAuth)
            {
                connectionStringBuilder.IntegratedSecurity = true;
            }
            else
            {
                connectionStringBuilder.UserID = username;
                connectionStringBuilder.Password = password;
            }

            var connectionString = connectionStringBuilder.ToString();
            var connection = new SqlConnection(connectionString);
            return connection;
        }
    }
}
