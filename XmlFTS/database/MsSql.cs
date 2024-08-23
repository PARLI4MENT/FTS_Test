using System.Data.SqlClient;
using System.IO;

namespace SQLNs
{
    internal class MsSql
    {
        private static string _Server { get; set; } = "192.168.0.142";
        public void SetServer(string _server) => _server = _Server;
        public string GetServer() => _Server;


        private static string _Database { get; set; } = "declarantplus";
        public string GetDatabase() => _Database;

        private static string _Uid { get; set; } = "SA";
        public void SetUid(string _uid) => _Uid = _uid;
        public string GetUid() => _Uid;

        private static string _Password { get; set; } = "P&sswd0105";
        public void SetPassword(string _password) => _Password = _password;
        public string GetPassword() => _Password;

        string _strConnMain = $"Server={_Server};Database={_Database};User Id={_Uid};Password={_Password};";

        public void ExecuteToDB(string[] args)
        {
            using (var connection = new SqlConnection(_strConnMain))
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    command.CommandText = $"INSERT [mainScheme].[ExchED] (\"InnerID\", \"MessageType\", \"EnvelopeID\", \"CompanySet_key_id\", " +
                        "\"DocumentID\", \"DocName\", \"DocNum\", \"DocCode\", \"ArchFileName\")" +
                        $" VALUES ('{args[0]}', 'CMN.00202', '{args[1]}', 1, '{args[2]}', '{args[3]}', '{args[4]}', '{args[5]}', '{Path.GetFileName(args[6])}')";
                    command.Connection = connection;
                    command.ExecuteNonQuery();
                }
            }
        }

        public void ExecuteToDB(string[] args, int Company_key_id)
        {

        }

    }
}