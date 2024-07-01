using Npgsql;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;

namespace PgSql
{
    public class PgSql
    {
        public static string Server { get; private set; } = "localhost";
        public static string Port { get; private set; } = "5432";
        public static string Database { get; private set; } = "testdb";
        public static string Uid { get; private set; } = "postgres";
        public static string Password { get; private set; } = "passwd0105";

        private static string _tableName = "testTable";

        string _strConnMain = $"Server={Server};Port={Port};Uid={Uid};Pwd={Password};";

        public PgSql() { }

        public PgSql(string Server, string Port, string Uid, string Password)
        {
            PgSql.Server = Server;
            PgSql.Port = Port;
            PgSql.Uid = Uid;
            PgSql.Password = Password;
        }

        public delegate void PgDataOut(string tableName, int iteration = 100);

        public async void PgSqlConnect()
        {
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(_strConnMain);
            var dataSource = dataSourceBuilder.Build();
            var sqlConnection = await dataSource.OpenConnectionAsync();

            if (sqlConnection.State == ConnectionState.Open)
                Console.WriteLine("State => is Open");
            else
                Console.WriteLine("State => wasn`t Open");
        }

        public async void PgSqlCreateDatabase(bool CreateTable = false)
        {
            string strComm = @$"CREATE DATABASE {Database} WITH OWNER postgres ENCODING = 'UTF8' CONNECTION LIMIT = -1;";

            await using (var sqlConn = new NpgsqlConnection(_strConnMain))
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
                    PgSqlCreateTable();
            }
        }

        public async void PgSqlCreateTable()
        {
            string strCreateTable = @$"CREATE TABLE ""public"".""{_tableName}"" (
                ""ID"" int4 NOT NULL GENERATED ALWAYS AS IDENTITY (INCREMENT 1), ""test1"" bool, ""test2"" char, ""test3"" varchar(255),
                ""test4"" decimal(10,2), ""test5"" float8, ""test6"" int8, ""test7"" text, ""test8"" varchar(255), ""test9"" varchar(255),
                PRIMARY KEY (""ID""));";

            try
            {
                await using (var sqlConn = new NpgsqlConnection(string.Concat(_strConnMain, "Database=", Database, ";")))
                {
                    await sqlConn.OpenAsync();
                    await using (var sqlComm = new NpgsqlCommand(strCreateTable, sqlConn))
                    {
                        await sqlComm.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                switch (ex.SqlState)
                {
                    // DuplicateTable
                    case "42P07":
#if DEBUG
                        Console.WriteLine($"[Msg] => {ex.Message}");
                        Console.WriteLine($"[Source] => {ex.Source}");
                        Console.WriteLine($"[Data] => {ex.Data}");
                        Console.WriteLine($"[SqlState] => {ex.SqlState}");
                        Console.WriteLine($"[BatchCommand] => {ex.BatchCommand}");
                        Console.WriteLine($"[IsTransient] => {ex.IsTransient}");
#endif
                        break;
                    // InvalidCatalogName
                    case "3D000":
#if DEBUG
                        Console.WriteLine($"[Msg] => {ex.Message}");
                        Console.WriteLine($"[Source] => {ex.Source}");
                        Console.WriteLine($"[Data] => {ex.Data}");
                        Console.WriteLine($"[SqlState] => {ex.SqlState}");
                        Console.WriteLine($"[BatchCommand] => {ex.BatchCommand}");
                        Console.WriteLine($"[IsTransient] => {ex.IsTransient}");
#endif
                        PgSqlCreateDatabase(true);
                        break;
                    default:
                        break;
                }
            }
        }

        public static bool PgSqlCheckDb(NpgsqlConnection sqlConnection, [Optional] string tableName)
        {
            // Get all tables in current Database
            try
            {
                #region Get tables
                tableName = "testTabel1";
                const string listTables = "SELECT table_name FROM information_schema.tables WHERE table_schema='public'";
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
                            //Console.WriteLine(reader.FieldCount.ToString());
                            Console.WriteLine($"[{i++}]\t{reader.GetString(0)}");
                            if (reader.GetString(0) == tableName)
                            {
#if DEBUG
                                Console.WriteLine($"Table is found {reader.GetString(0)}");
                                return true;
#endif
                            }
                        }
                    }
                    return false;
                }
                #endregion
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); return false; }
        }

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

        private async static void PgRetriveData(NpgsqlConnection sqlConnection)
        {
            await using (var sqlComm = new NpgsqlCommand("SELECT * FROM \"public\".\"testTabel1\"", sqlConnection))
            {
                await using (NpgsqlDataReader reader = await sqlComm.ExecuteReaderAsync())
                {
                    if (!reader.HasRows)
                    {
#if DEBUG
                        Console.WriteLine("No rows!");
#endif
                        return;
                    }

                    while (await reader.ReadAsync())
                    {
                        Console.WriteLine(reader.GetString(0));
                    }
                }
            }
        }

        private async void PgClearData([Optional] string _tableName)
        {
#if DEBUG
            var sw = new Stopwatch();
            sw.Start();
#endif
            string strCommand = $@"DELETE FROM""public"".""{_tableName}""";

            await using (var sqlConnection = new NpgsqlConnection(string.Concat(_strConnMain, "Database=", Database, ";")))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    await using (var sqlCommand = new NpgsqlCommand(strCommand, sqlConnection))
                        sqlCommand.ExecuteNonQuery();
                }
                catch (NpgsqlException ex)
                {
                    switch (ex.SqlState)
                    {
                        //Npgsql.PostgresException: '42P01: relation "public.testTable" does not exist
                        case "42P01":
                            PgSqlCreateTable();
                            break;
                        case "3D000":
                            PgSqlCreateTable();
                            break;
                        default:
                            break;
                    }
                    await Console.Out.WriteLineAsync(ex.Message);
                    return;
                }
                finally
                {
                    if (sqlConnection.State == ConnectionState.Open)
                        sqlConnection.Close();
                }
            }
#if DEBUG
            sw.Stop();
            Console.WriteLine($"\nClear data from {_tableName}");
            Console.WriteLine($"Total time => {sw.ElapsedMilliseconds / 1000} sec ({sw.ElapsedMilliseconds} ms)");
#endif
        }
    }
}