using Npgsql;
using System;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using XmlFTS.OutClass;

namespace SQLNs
{
    public class PgSql
    {
        private static string connectionKey = "PgConnectionString";

        private static string _Server = "192.168.0.142";
        public void SetServer(string _server) => _Server = _server;

        private static string _Port = "5438";
        public void SetPort(string _port) => _Port = _port;

        private static string _Uid = "postgres";
        public void SetUid(string _uid) => _Uid = _uid;

        private static string _Password = "passwd0105";
        public void SetPassword(string _password) => _Password = _password;

        private static string _Database = "declarantplus";
        public string SetDatabase(string _database) => _Database = _database;

        public static string ConnectionString { get; private set; }

        public PgSql()
        {
            //if (!string.IsNullOrEmpty(Config.ReadSettings(connectionKey)))
            //    ConnectionString = Config.ReadSettings(connectionKey);
        }

        public PgSql(string Server, string Port, string Uid, string Password, [Optional]string Database)
        {
            _Server = Server;
            _Port = Port;
            _Uid = Uid;
            _Password = Password;

            SetConnectionString(Server, Port, Uid, Password, Database);

            PgSqlCheckConnection();
        }

        public static void SetConnectionString(string Server, string Port, string Uid, string Password, [Optional]string Database)
        {
            if (string.IsNullOrEmpty(Server) || string.IsNullOrEmpty(Port) || string.IsNullOrEmpty(Uid) || string.IsNullOrEmpty(Password))
            {
                Debug.WriteLine("Один из входящих параметров пуст или null");
                return;
            }

            /// Сделать If с тестовым соединением и последующим сохранением в статические поля и файл конфигурации
            if (string.IsNullOrEmpty(Database))
            {
                Config.AddUpdateAppSettings(connectionKey, $"Server={_Server};Port={_Port};Uid={_Uid};Pwd={_Password};");
                ConnectionString = Config.ReadSettings(connectionKey);
                return;
            }
                Config.AddUpdateAppSettings(connectionKey, string.Concat(ConfigurationManager.ConnectionStrings[connectionKey].ConnectionString, "Database=", _Database, ";"));
        }
        
        public void SetConnectionString()
        {
            Config.AddUpdateAppSettings(connectionKey, $"Server={_Server};Port={_Port};Uid={_Uid};Pwd={_Password};");
        }

        /// <summary> Выполнение запроса в PostgresSQL </summary>
        /// <remarks>
        /// string[0] NameArray, string[1] EnvelopeID, string[2] DocumentID, string[3] PrDocumentName
        /// string[4] PrDocumentNumber, string[5] DocCode, string[6] NewDocToArchName
        /// </remarks>
        /// <param name="args"> Массив значение типа string </param>
        /// <param name="Company_key_id">Уникальный ID компании</param>
        public void ExecuteToDB(string[] args, int Company_key_id)
        {
            //using (var sqlConn = new NpgsqlConnection(ConfigurationManager.ConnectionStrings[connectionKey].ConnectionString))
            using (var sqlConn = new NpgsqlConnection($"Server=192.168.0.142;Port=5438;Uid=postgres;Pwd=passwd0105;Database=declarantplus;"))
            {
                sqlConn.Open();
                using (var sqlComm = new NpgsqlCommand())
                {
                    sqlComm.CommandText = $@"INSERT INTO ""public"".""ExchED""
                                (""InnerID"", ""MessageType"", ""EnvelopeID"", ""CompanySet_key_id"",
                                ""DocumentID"", ""DocName"", ""DocNum"", ""DocCode"", ""ArchFileName"")
                                VALUES ('{args[0]}', 'CMN.00202', '{args[1]}', {Company_key_id}, '{args[2]}',
                                '{args[3]}', '{args[4]}', '{args[5]}', '{Path.GetFileName(args[6])}');";
                    sqlComm.Connection = sqlConn;
                    sqlComm.ExecuteNonQuery();
                    // ArchivePathDoc ???
                }
                sqlConn.Close();
            }
        }

        /// <summary> Выполнение запроса в PostgresSQL </summary>
        /// <remarks>
        /// string[0] NameArray, string[1] EnvelopeID, string[2] DocumentID, string[3] PrDocumentName
        /// string[4] PrDocumentNumber, string[5] DocCode, string[6] NewDocToArchName
        /// </remarks>
        /// <param name="args"> Массив значение типа string </param>
        public void ExecuteToDB(string[] args)
        {
            try
            {
                //using (var sqlConn = new NpgsqlConnection(ConfigurationManager.ConnectionStrings[connectionKey].ConnectionString))
                using (var sqlConn = new NpgsqlConnection($"Server=192.168.0.142;Port=5438;Uid=postgres;Pwd=passwd0105;Database=declarantplus;"))
                {
                    sqlConn.Open();
                    using (var sqlComm = new NpgsqlCommand())
                    {
                        sqlComm.CommandText = $@"INSERT INTO ""public"".""ExchED""
                                (""InnerID"", ""MessageType"", ""EnvelopeID"", ""CompanySet_key_id"",
                                ""DocumentID"", ""DocName"", ""DocNum"", ""DocCode"", ""ArchFileName"")
                                VALUES ('{args[0]}', 'CMN.00202', '{args[1]}', 1, '{args[2]}',
                                '{args[3]}', '{args[4]}', '{args[5]}', '{Path.GetFileName(args[6])}');";
                        sqlComm.Connection = sqlConn;
                        sqlComm.ExecuteNonQuery();
                        // ArchivePathDoc ???
                    }
                    sqlConn.Close();
                }
            }
            catch (PostgresException pEx) { Debug.WriteLine(pEx.Message); }
        }

        public delegate void PgDataOut(string tableName, int iteration = 100);

        private static bool PgSqlCheckConnection()
        {
            try
            {

                using (var sqlConnection = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["PgConnectionString"].ConnectionString))
                {
                    sqlConnection.Open();
                    if (sqlConnection.State == ConnectionState.Open)
                    {
                        Console.WriteLine("Connection state => is Open");
                        return true;
                    }
                    else
                        Console.WriteLine("Connection state => wasn`t Open");

                    Debug.WriteLine($"\nServer => {_Server}");
                    Debug.WriteLine($"Port => {_Port}");
                    Debug.WriteLine($"Uid => {_Uid}");
                    Console.WriteLine($"Password => {_Password}");
                    return false;
                }
            }
            catch (Exception) { return false; }
        }

        private bool CheckDatabaseExist()
        {
            string _strConnMain = $"Server={_Server};Port={_Port};Uid={_Uid};Pwd={_Password};";

            using (var sqlConnection = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["PgConnectionString"].ConnectionString))
            {
                using (var sqlCommand = new NpgsqlCommand()) { }
            }

            return false;
        }

        private void PgSqlCreateDatabase(bool CreateTable = false)
        {
            string strComm = $@"CREATE DATABASE {_Database} WITH OWNER postgres ENCODING = 'UTF8' CONNECTION LIMIT = -1;";

            using (var sqlConn = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["PgConnectionString"].ConnectionString))
            {
                Debug.WriteLine(sqlConn.State.ToString());
                sqlConn.Open();

                Debug.WriteLine(sqlConn.State.ToString());
                var sqlComm = new NpgsqlCommand(strComm, sqlConn);
                sqlComm.ExecuteNonQuery();

                Debug.WriteLine("Query is Done!");
                sqlConn.Close();

                Debug.WriteLine(sqlConn.State.ToString());

                if (CreateTable)
                    PgSqlBaseCreateTable();
            }

            /// Npgsql.PostgresException: '42P04: database "declarantplus" already exists'
        }

        public void PgSqlBaseCreateTable()
        {
            try
            {
                using (var sqlConn = new NpgsqlConnection(string.Concat(ConfigurationManager.ConnectionStrings["PgConnectionString"].ConnectionString, "Database=", _Database, ";")))
                {
                    sqlConn.Open();
                    /// Create table => ECD_list
                    using (var sqlComm = new NpgsqlCommand(Fields.ECD_list.GetSqlCommandCreator, sqlConn))
                        sqlComm.ExecuteNonQuery();

                    /// Create table => ExchED
                    using (var sqlComm = new NpgsqlCommand(Fields.ExchED.GetSqlCommandCreator, sqlConn))
                        sqlComm.ExecuteNonQuery();

                    sqlConn.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary> NOT A FINISHTED </summary>
        /// <param name="sqlConnection"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static bool PgSqlCheckDb(NpgsqlConnection sqlConnection, [Optional] string tableName)
        {
            // Get all tables in current Database
            try
            {
                #region Get tables
                tableName = "testTabel1";
                const string listTables = "SELECT * FROM information_schema.tables WHERE table_schema='public'";
                using (var sqlComm = new NpgsqlCommand(listTables, sqlConnection))
                {
                    using (var reader = sqlComm.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
#if DEBUG
                            Console.WriteLine("No tables!");
#endif
                            return false;
                        }
                        int i = 0;
                        while (reader.Read())
                        {
#if DEBUG
                            Console.WriteLine($"[{i++}]\t{reader.GetString(0)}");
#endif
                            if (reader.GetString(0) == tableName)
                            {
#if DEBUG
                                Console.WriteLine($"Table is found {reader.GetString(0)}");
#endif
                                return true;
                            }
                        }
                    }
                    return false;
                }
                #endregion
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); return false; }
        }

        ///<summary> Получение данных из </summary>
        ///<param name="searchValue">Значение, которое ищем</param>
        /// <param name="destEnvelopeID">Значение, которое будет "вставлено", в случае, если строка со значением существует</param>
        /// <returns></returns>
        public void PgRetriveData(string searchEnvelopeID, string destEnvelopeID, string status)
        {
            try
            {
                using (var sqlConn = new NpgsqlConnection($"Server=192.168.0.142;Port=5438;Uid=postgres;Pwd=passwd0105;Database=declarantplus;"))
                {
                    sqlConn.Open();
                    string strCommand = $@"SELECT COUNT(*) FROM ""public"".""ExchED"" WHERE ""EnvelopeID""='{searchEnvelopeID}';";
                    using (var sqlComm = new NpgsqlCommand(strCommand, sqlConn))
                    {
                        var ExchEDCount = (Int64)sqlComm.ExecuteScalar();
                        strCommand = $@"SELECT COUNT(*) FROM ""public"".""ECD_list"" WHERE ""InnerID""='{searchEnvelopeID}';";
                        sqlComm.CommandText = strCommand;
                        var ECD_listCount = (Int64)sqlComm.ExecuteScalar();
                        if (ExchEDCount > 0 && ECD_listCount == 0)
                        {
                            sqlComm.CommandText = $@"INSERT INTO ""public"".""ECD_list""
                                (""InnerID"", ""Status"", ""DocsSended"")
                                VALUES ('{searchEnvelopeID}', '{status}', 1);";
                            sqlComm.ExecuteNonQuery();
                            sqlConn.Close();
                        }
                        else
                            Debug.WriteLine("Запись существует");
                        sqlConn.Close();
                    }
                }
            }
            catch (NpgsqlException npgEx) { Debug.WriteLine(npgEx.Message); }
        }

        /// <summary> Полная очистка таблицы от данных PostgresSql</summary>
        /// <param name="_tableName"></param>
        private void PgClearData([Optional] string _tableName)
        {
            string strCommand = $@"DELETE FROM""public"".""{_tableName}""";

            using (var sqlConnection = new NpgsqlConnection(string.Concat(ConfigurationManager.ConnectionStrings["PgConnectionString"].ConnectionString, "Database=", _Database, ";")))
            {
                try
                {
                    sqlConnection.OpenAsync();
                    using (var sqlCommand = new NpgsqlCommand(strCommand, sqlConnection))
                        sqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return;
                }
                finally
                {
                    if (sqlConnection.State == ConnectionState.Open)
                        sqlConnection.Close();
                }
            }
        }
    }
}