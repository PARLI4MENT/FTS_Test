using Microsoft.VisualBasic;
using Npgsql;
using System;
using System.Runtime.CompilerServices;
using System.Configuration;

namespace mainNS
{
    public class Program
    {
        const string pgStrConn = "Server=192.168.0.142;Port=5438;Database=student;User ID=postgres;Password=passwd0105";



        public static void Main(string[] args)
        {
            PGConnect();

            Console.ReadKey();
        }

        public async static void PGConnect()
        {
            await using var sqlConnection = new NpgsqlConnection(pgStrConn);
            await sqlConnection.OpenAsync();

            if (sqlConnection.State == System.Data.ConnectionState.Open)
                Console.WriteLine("State => is Open");
            else
                Console.WriteLine("State => wasn`t Open");

            //InsertData(conn);
            //RetriveRow(conn);

            DataReader(sqlConnection);
        }

        private async static void DataReader(NpgsqlConnection sqlConnection)
        {
            try
            {

            }
            catch (Exception ex) { Console.WriteLine(ex.Message); return; }

            throw new Exception();
        }


        //private async static void InsertData(NpgsqlConnection sqlConnection)
        //{
        //    //await using (var command = new NpgsqlCommand("INSERT INTO Student (FIO_client, Number_client, Record ) VALUES ('Test1', 'Test1', 'Test' );", sqlConnection))
        //    await using (var command = new NpgsqlCommand("INSERT INTO testDB (Name) VALUES (\"Test 1\");", sqlConnection))
        //    {
        //        //var params[] = new NpgsqlParameter("fio", "test");
        //        //await command.Parameters.Add();
        //        await command.ExecuteNonQueryAsync();
        //    }
        //}

        //private async static void RetriveRow(NpgsqlConnection sqlConnection)
        //{
        //    await using (var command = new NpgsqlCommand("SELECT * FROM Student", sqlConnection))
        //    {
        //        await using (var reader = await command.ExecuteReaderAsync())
        //        {
        //            while (await reader.ReadAsync())
        //            {
        //                Console.WriteLine(reader.GetString(0));
        //            }
        //        }
        //    }
        //}
    }
}