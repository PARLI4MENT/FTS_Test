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
            PGConnect();

            Console.ReadKey();
        }

        public async static void PGConnect()
        {
            await using var sqlConnection = new NpgsqlConnection(strConn);
            await sqlConnection.OpenAsync();

            if (sqlConnection.State == System.Data.ConnectionState.Open)
                Console.WriteLine("State => is Open");
            else
                Console.WriteLine("State => wasn`t Open");

            DataReader(sqlConnection);
            //RetriveRow(conn);
            //InsertData(conn);
        }

        private async static void DataReader(NpgsqlConnection sqlConnection)
        {
            try
            {
                NpgsqlCommand sqlComm = new NpgsqlCommand();
                sqlComm.Connection = sqlConnection;
                sqlComm.CommandType = System.Data.CommandType.Text;
                sqlComm.CommandText = "SELECT * FROM \"public\".\"Student\"";

                NpgsqlDataReader dataReader = sqlComm.ExecuteReader();

                if (dataReader.HasRows)
                {
                    DataTable dataTable = new DataTable();
                    dataTable.Load(dataReader);
                }

                sqlComm.Dispose();
                sqlConnection.Close();                
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); return; }
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