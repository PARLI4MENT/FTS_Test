using Npgsql;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using XMLSigner.SQL;

namespace SQLNs
{
    public class PgSql
    {
        private static string _Server { get; set; } = "192.168.0.142";
        public void SetServer(string _server) => _server = _Server;
        public string GetServer() => _Server;

        private static string _Port { get; set; } = "5438";
        public void SetPort(string _port) => _Port = _port;
        public string GetPort() => _Port;

        private static string _Database { get; set; } = "declarantplus";
        public string GetDatabase() => _Database;

        private static string _Uid { get; set; } = "postgres";
        public void SetUid(string _uid) => _Uid = _uid;
        public string GetUid() => _Uid;

        private static string _Password { get; set; } = "passwd0105";
        public void SetPassword(string _password) => _Password = _password;
        public string GetPassword() => _Password;

        public static NpgsqlConnection _pgConnection;

        string _strConnMain = $"Server={_Server};Port={_Port};Uid={_Uid};Pwd={_Password};";

        public PgSql() { }

        public PgSql(string Server, string Port, string Uid, string Password)
        {
            _Server = Server;
            _Port = Port;
            _Uid = Uid;
            _Password = Password;
            if (PgSqlCheckConnection())
                Console.WriteLine("Error connection to PostgresSQL database");

        }

        public void ExecuteToDB(string[] args, int Company_key_id)
        {
            using (var sqlConn = new NpgsqlConnection(_strConnMain))
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

        /// <summary> NotImplementedException </summary>
        /// <param name="args"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void ExecuteToDB(string[] args)
        {
            using (var sqlConn = new NpgsqlConnection(_strConnMain))
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

        public delegate void PgDataOut(string tableName, int iteration = 100);

        private static bool PgSqlCheckConnection()
        {
            string _strConnMain = $"Server={_Server};Port={_Port};Uid={_Uid};Pwd={_Password};";
            try
            {

                using (var sqlConnection = new NpgsqlConnection(_strConnMain))
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

        public bool CheckDatabaseExist()
        {
            string _strConnMain = $"Server={_Server};Port={_Port};Uid={_Uid};Pwd={_Password};";

            using (var sqlConnection = new NpgsqlConnection(_strConnMain))
            {
                using (var sqlCommand = new NpgsqlCommand())
                {

                }
            }

            return false;
        }

        public void PgSqlCreateDatabase(bool CreateTable = false)
        {
            string strComm = $@"CREATE DATABASE {_Database} WITH OWNER postgres ENCODING = 'UTF8' CONNECTION LIMIT = -1;";

            using (var sqlConn = new NpgsqlConnection(_strConnMain))
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
            /// NOT USE ONLY FOR TEST!
            /*
            string strCreateTable = @$"CREATE TABLE ""public"".""{_tableName}"" (
                ""ID"" int4 NOT NULL GENERATED ALWAYS AS IDENTITY (INCREMENT 1), ""test1"" bool, ""test2"" char, ""test3"" varchar(255),
                ""test4"" decimal(10,2), ""test5"" float8, ""test6"" int8, ""test7"" text, ""test8"" varchar(255), ""test9"" varchar(255),
                PRIMARY KEY (""ID""));";
            */

            try
            {
                using (var sqlConn = new NpgsqlConnection(string.Concat(_strConnMain, "Database=", _Database, ";")))
                {
                    sqlConn.Open();
                    /// Create table => ECD_list
                    using (var sqlComm = new NpgsqlCommand(Fields.ECD_list.GetSqlCommandCreator, sqlConn))
                        sqlComm.ExecuteNonQuery();

                    /// Create table => ExchED
                    using (var sqlComm = new NpgsqlCommand(Fields.ExchED.GetSqlCommandCreator, sqlConn))
                        sqlComm.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// NOT A FINISHTED
        /// </summary>
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

        #region TEST INSERT && INSERT_PARALLEL
        /*
        public async void PgInsertData(int iteration = 100)
        {
            PgClearData(_tableName);
#if DEBUG
            var sw = new Stopwatch();
            sw.Start();
#endif
            try
            {
                string strCommand = @$"INSERT INTO ""public"".""{_tableName}"" (
                        test1, test2, test3, test4, test5, test6, test7, test8, test9)
                        VALUES ({true}, 'c', {DateTime.Now.ToString("yyyy-MM-dd")}, 3.14, 3.14, 1, 'some_text', 'some_text', 'some_text');";

                await using (var sqlConnection = new NpgsqlConnection(string.Concat(_strConnMain, "Database=", Database, ";")))
                {
                    await sqlConnection.OpenAsync();
                    int i = 0;
                    await using (var sqlCommand = new NpgsqlCommand(strCommand, sqlConnection))
                    {
                        while (i < iteration)
                        {
                            await sqlCommand.ExecuteNonQueryAsync();
                            i++;
                        }
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); return; }
#if DEBUG
            sw.Stop();
            Console.WriteLine("\nAsync requests:");
            Console.WriteLine($"Requests => {iteration} units");
            Console.WriteLine($"Total time => {sw.ElapsedMilliseconds / 1000} sec ({sw.ElapsedMilliseconds} ms)");
            Console.WriteLine($"Average => {(long)iteration / (sw.ElapsedMilliseconds / 1000)} q/s");
#endif
        }

        public void PgInsertDataParallel(int iteration)
        {
            #region
            PgClearData();
            #endregion
#if DEBUG
            var sw = new Stopwatch();
            sw.Start();
#endif
            string strCommand = @$"INSERT INTO ""public"".""{_tableName}"" (
                        test1, test2, test3, test4, test5, test6, test7, test8, test9)
                        VALUES ({true}, 'c', {DateTime.Now.ToString("yyyy-MM-dd")}, 3.14, 3.14, 1, 'some_text', 'some_text', 'some_text');";

            Parallel.For(0, iteration, i =>
            {
                using (var sqlConnection = new NpgsqlConnection(string.Concat(_strConnMain, "Database=", Database, ";")))
                {
                    sqlConnection.Open();
                    using (var sqlCommand = new NpgsqlCommand(strCommand, sqlConnection))
                        sqlCommand.ExecuteNonQuery();
                }
            });
#if DEBUG
            sw.Stop();
            Console.WriteLine("\nParallels requests:");
            Console.WriteLine($"Requests => {iteration} units");
            Console.WriteLine($"Total time => {sw.ElapsedMilliseconds / 1000} sec ({sw.ElapsedMilliseconds} ms)");
            Console.WriteLine($"Average => {(long)iteration / (sw.ElapsedMilliseconds / 1000)} q/s");
#endif
        }
        */
        #endregion

        /// <summary> Получение данных из </summary>
        /// <param name="sqlConnection">Объект подключения</param>
        private static void PgRetriveData(NpgsqlConnection sqlConnection)
        {
            using (var sqlComm = new NpgsqlCommand("SELECT * FROM \"public\".\"testTabel1\"", sqlConnection))
            {
                using (NpgsqlDataReader reader = sqlComm.ExecuteReader())
                {
                    if (!reader.HasRows)
                        return;

                    while (reader.Read())
                        Console.WriteLine(reader.GetString(0));
                }
            }
        }

        /// <summary> Полная очистка таблицы от данных PostgresSql</summary>
        /// <param name="_tableName"></param>
        private void PgClearData([Optional] string _tableName)
        {
            string strCommand = $@"DELETE FROM""public"".""{_tableName}""";

            using (var sqlConnection = new NpgsqlConnection(string.Concat(_strConnMain, "Database=", _Database, ";")))
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