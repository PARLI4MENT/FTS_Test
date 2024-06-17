using Microsoft.VisualBasic;
using Npgsql;
using System;
using System.Runtime.CompilerServices;

namespace mainNS
{
    public class Program
    {
        const string strConnection = "Server=192.168.0.142;Port=5438;Database=student;User ID=postgres;Password=passwd0105";

        public static void Main(string[] args)
        {
            PGConnect();

            Console.ReadKey();
        }

        public async static void PGConnect()
        {
            await using var conn = new NpgsqlConnection(strConnection);
            await conn.OpenAsync();

            if (conn.State == System.Data.ConnectionState.Open)
                Console.WriteLine("State => is Open");
            else
                Console.WriteLine("State => wasn`t Open");

            InsertData(conn);
            //RetriveRow(conn);
        }

        private async static void InsertData(NpgsqlConnection sqlConnection)
        {
            //await using (var command = new NpgsqlCommand("INSERT INTO Student (FIO_client, Number_client, Record ) VALUES ('Test1', 'Test1', 'Test' );", sqlConnection))
            await using (var command = new NpgsqlCommand("INSERT INTO testDB (Name) VALUES (\"Test 1\");", sqlConnection))
            {
                //var params[] = new NpgsqlParameter("fio", "test");
                //await command.Parameters.Add();
                await command.ExecuteNonQueryAsync();
            }
        }

        private async static void RetriveRow(NpgsqlConnection sqlConnection)
        {
            await using (var command = new NpgsqlCommand("SELECT * FROM Student", sqlConnection))
            {
                await using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Console.WriteLine(reader.GetString(0));
                    }
                }
            }
        }
    }
}