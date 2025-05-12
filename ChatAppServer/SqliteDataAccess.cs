using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ChatAppServer.Server;

namespace ChatAppServer
{
    public class SqliteDataAccess
    {

        public static List<accountModel> loadPeople()
        {

            using (IDbConnection conn = new SQLiteConnection(LoadConnectionStrin()))
            {
                var output = conn.Query<accountModel>("select * from accounts", new DynamicParameters());
                return output.ToList();

            }

        }

        public static void SaveAccount(accountModel account)
        {

            using (IDbConnection conn = new SQLiteConnection(LoadConnectionStrin()))
            {
                conn.Execute("insert into accounts (username, password, profPic, status) values (@username, @password, @profPic, @status)", account);

            }

        }

        private static string LoadConnectionStrin(string id = "Default")
        {

            return ConfigurationManager.ConnectionStrings[id].ConnectionString;

        }


    }
}
 