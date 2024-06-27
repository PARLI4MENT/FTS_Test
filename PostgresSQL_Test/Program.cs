﻿using Microsoft.VisualBasic;
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
            //PgSqlCreateTable();

            //PgSqlCreateTable();
            var sw = new Stopwatch();
            sw.Start();
            PgInsertData("testTabel1");
            sw.Stop();
            Console.WriteLine(sw.Elapsed);

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
                ""ID"" int4 NOT NULL GENERATED ALWAYS AS IDENTITY (INCREMENT 1), ""test1"" bool, ""test2"" char, ""test3"" varchar(255),
                ""test4"" decimal(10,2), ""test5"" float8, ""test6"" int8, ""test7"" text, ""test8"" varchar(255), ""test9"" varchar(255),
                PRIMARY KEY (""ID""));";

            try
            {
                await using (var sqlConn = new NpgsqlConnection(strConnMain))
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
                    case "42P07":
#if DEBUG
                        //Console.WriteLine($"[Code] => {ex.ErrorCode}");
                        Console.WriteLine($"[Msg] => {ex.Message}");
                        Console.WriteLine($"[Source] => {ex.Source}");
                        //Console.WriteLine($"[StackTrace] => {ex.StackTrace}");
                        //Console.WriteLine($"[HResult] => {ex.HResult}");
                        Console.WriteLine($"[Data] => {ex.Data}");
                        Console.WriteLine($"[SqlState] => {ex.SqlState}");
                        Console.WriteLine($"[BatchCommand] => {ex.BatchCommand}");
                        Console.WriteLine($"[IsTransient] => {ex.IsTransient}");
#endif
                        break;
                    case "3D000":
#if DEBUG
                        //Console.WriteLine($"[Code] => {ex.ErrorCode}");
                        Console.WriteLine($"[Msg] => {ex.Message}");
                        Console.WriteLine($"[Source] => {ex.Source}");
                        //Console.WriteLine($"[StackTrace] => {ex.StackTrace}");
                        //Console.WriteLine($"[HResult] => {ex.HResult}");
                        Console.WriteLine($"[Data] => {ex.Data}");
                        Console.WriteLine($"[SqlState] => {ex.SqlState}");
                        Console.WriteLine($"[BatchCommand] => {ex.BatchCommand}");
                        Console.WriteLine($"[IsTransient] => {ex.IsTransient}");
#endif
                        PgSqlCreateDatabase();
                        break;
                    default:
                        break;
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

        private async static void PgInsertData(NpgsqlConnection sqlConnection, string tableName, int min = 0, int iteration = 100)
        {
            try
            {
                await using (var sqlComm = new NpgsqlCommand($"INSERT", sqlConnection))
                {
                    string strComm = @$"INSERT INTO ""public"".""{tableName}"" (
                        test1, test2, test3, test4, test5, test6, test7, test8, test9)
                        VALUES ({true}, 'c', {DateTime.Now.ToString("yyyy-MM-dd")}, 3.14, 3.14, 1, 'some_text', 'some_text', 'some_text');";
#if DEBUG
                    Console.WriteLine("[DEBUG] => ");
#endif
                }

            }
            catch (Exception ex) { Console.WriteLine(ex.Message); return; }
        }

        private async static void PgInsertData(string tableName, int min = 0, int iteration = 100)
        {
            try
            {
                if (min >= iteration)
                    return;
                string strComm = @$"INSERT INTO ""public"".""{tableName}"" (
                        test1, test2, test3, test4, test5, test6, test7, test8, test9)
                        VALUES ({true}, 'c', {DateTime.Now.ToString("yyyy-MM-dd")}, 3.14, 3.14, 1, 'some_text', 'some_text', 'some_text');";

                await using (var sqlConn = new NpgsqlConnection(strConnMain))
                {
                    await sqlConn.OpenAsync();
                    await using (var sqlComm = new NpgsqlCommand(strComm, sqlConn))
                    {
                        await sqlComm.ExecuteNonQueryAsync();
#if DEBUG
                        Console.WriteLine("[DEBUG] => Execute {} is Done!");
#endif
                    }
                }
                min++;
                PgInsertData(tableName, min, iteration);
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