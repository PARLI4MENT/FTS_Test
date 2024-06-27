using Microsoft.VisualBasic;
using Npgsql;
using System.Buffers;
using System.Data;
using System.Diagnostics;

namespace MainNS
{
    public class Program
    {
        static string strConnMain = "Server=192.168.0.142;Port=5438;Database=testdb;Uid=postgres;Pwd=passwd0105";

        public static void Main(string[] args)
        {
            //PgSqlConnect();
            //PgSqlCreateDatabase();
            PgSqlCreateTable();

            Console.ReadKey();
        }

        public async static void PgSqlConnect()
        {

            // Connection
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(strConnMain);
            var dataSource = dataSourceBuilder.Build();
            var sqlConnection = await dataSource.OpenConnectionAsync();

            if (sqlConnection.State == ConnectionState.Open)
                Console.WriteLine("State => is Open");
            else
                Console.WriteLine("State => wasn`t Open");

            //PgCheckDb(sqlConnection);

            PgRetriveData(sqlConnection);
            //PgInsertData(sqlConnection);
        }


        public async static void PgSqlCreateDatabase()
        {
            string strDbName = "testDB";
            string strConn = "Server=192.168.0.142;Port=5438;Uid=postgres;Pwd=passwd0105;";
            string strComm = @$"CREATE DATABASE {strDbName} WITH OWNER postgres ENCODING = 'UTF8' CONNECTION LIMIT = -1;";

            await using (var sqlConn = new NpgsqlConnection(strConn))
            {

                Debug.WriteLine(sqlConn.State.ToString());
                sqlConn.Open();
                Debug.WriteLine(sqlConn.State.ToString());
                var sqlComm = new NpgsqlCommand(strComm, sqlConn);
                sqlComm.ExecuteNonQuery();
                Debug.WriteLine("Query is Done!");
                sqlConn.Close();
                Debug.WriteLine(sqlConn.State.ToString());

                PgSqlCreateTable();
            }

        }

        public async static void PgSqlCreateTable()
        {
            string strCreateTable = @"CREATE TABLE ""public"".""testTabel1"" (
                ""ID"" int4 NOT NULL GENERATED ALWAYS AS IDENTITY (INCREMENT 1), ""test1"" bool, ""test2"" char, ""test3"" date,
                ""test4"" decimal(10,2), ""test5"" float8, ""test6"" int8, ""test7"" text, ""test8"" varchar(255), ""test9"" varchar(255),
                PRIMARY KEY (""ID""));";

            await using (var sqlConn = new NpgsqlConnection(strConnMain))
            {
                await sqlConn.OpenAsync();
                await using (var sqlComm = new NpgsqlCommand(strCreateTable, sqlConn))
                {
                    await sqlComm.ExecuteNonQueryAsync();
                }
            }
        }

        private async static void PgSqlCheckDb(NpgsqlConnection sqlConnection, string tableName = "Student")
        {
            // Get all tables in current Database
            try
            {
                #region Get tables
                
                const string listTables = "SELECT table_name FROM information_schema.tables WHERE table_schema='public'";
                await using (var sqlComm = new NpgsqlCommand(listTables, sqlConnection))
                {
                    await using (var reader = sqlComm.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
#if DEBUG
                            Console.WriteLine("No tables!");
#endif
                            return;
                        }
                        int i = 0;
                        while (await reader.ReadAsync())
                        {
                            //Console.WriteLine(reader.FieldCount.ToString());
                            Console.WriteLine($"[{i++}]\t{reader.GetString(0)}");
                        }

                    }
                }
                #endregion

                #region Get fields from current table

                string fildsTable = $"SELECT column_name, data_type FROM information_schema.columns WHERE table_schema = 'public' AND table_name = '{tableName}'";
                await using (var sqlComm = new NpgsqlCommand(fildsTable, sqlConnection))
                {
                    await using (var reader = sqlComm.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
#if DEBUG
                            Console.WriteLine("No filds!");
#endif
                            return;
                        }
                        int i = 0;
                        while (reader.Read())
                        {
                            Console.WriteLine($"[{i++}]\t{reader.GetString(0)} => {reader.GetString(1)}");
                        }
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); return ; }
            #endregion
        }

        private async static void PgInsertData(NpgsqlConnection sqlConnection, string tableName, int iteration)
        {
            try
            {
                // InsertData
                await using (var sqlComm = new NpgsqlCommand("SELECT * FROM \"public\".\"Student\"", sqlConnection))
                {

                }

            }
            catch (Exception ex) { Console.WriteLine(ex.Message); return; }
        }

        private async static void PgRetriveData(NpgsqlConnection sqlConnection)
        {
            await using (var sqlComm = new NpgsqlCommand("SELECT * FROM \"public\".\"Student\"", sqlConnection))
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

    }
}