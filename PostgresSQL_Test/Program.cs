using Microsoft.VisualBasic;
using Npgsql;
using System;
using System.Runtime.CompilerServices;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace MainNS
{
    public class Program
    {
        static string? strConn = "Server=192.168.0.142;Port=5438;Database=student;Uid=postgres;Pwd=passwd0105";

        public static void Main(string[] args)
        {
            PgConnect();

            Console.ReadKey();
        }

        public async static void PgConnect()
        {
            // Connection
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(strConn);
            var dataSource = dataSourceBuilder.Build();
            var sqlConnection = await dataSource.OpenConnectionAsync();

            if (sqlConnection.State == System.Data.ConnectionState.Open)
                Console.WriteLine("State => is Open");
            else
                Console.WriteLine("State => wasn`t Open");


            //PgCheckDb(sqlConnection);

            PgRetriveData(sqlConnection);
            //PgInsertData(sqlConnection);
        }


        private async static void PgCheckDb(NpgsqlConnection sqlConnection, string tableName = "Student")
        {
            // Get all tables in current Database
            const string listTables = "SELECT table_name FROM information_schema.tables WHERE table_schema='public'";

            try
            {
                #region Get tables
                //                await using (var sqlComm = new NpgsqlCommand(listTables, sqlConnection))
                //                {
                //                    await using (var reader = sqlComm.ExecuteReader())
                //                    {
                //                        if (!reader.HasRows)
                //                        {
                //#if DEBUG
                //                            Console.WriteLine("No tables!");
                //#endif
                //                            return;
                //                        }
                //                        int i = 0;
                //                        while (await reader.ReadAsync())
                //                        {
                //                            //Console.WriteLine(reader.FieldCount.ToString());
                //                            Console.WriteLine($"[{i++}]\t{reader.GetString(0)}");
                //                        }

                //                    }
                //                }
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

        private async static void PgInsertData(NpgsqlConnection sqlConnection)
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
                        Console.WriteLine("No rows!");
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