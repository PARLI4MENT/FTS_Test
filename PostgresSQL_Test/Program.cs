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


            PgCheckDb(sqlConnection);

            //RetriveData(sqlConnection);
            //InsertData(sqlConnection);
        }


        private async static void PgCheckDb(NpgsqlConnection sqlConnection)
        {
            const string tableInfo = "SELECT * FROM information_schema.tables";

            try
            {
                await using (var sqlComm = new NpgsqlCommand(tableInfo, sqlConnection))
                {
                    await using (var reader = sqlComm.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            Console.WriteLine("Now tables");
                            return;
                        }
                        while (await reader.ReadAsync())
                        {
                            Console.WriteLine(reader.GetString(0));
                        }

                    }
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); return ; }

        }

        private async static void PgInsertData(NpgsqlConnection sqlConnection)
        {
            try
            {
                // Insert
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