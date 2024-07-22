using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using System.IO;
using System.Runtime.CompilerServices;

namespace SQLTestNs
{
    public class AccessDB
    {
        private static string pathToMDB;
        public static string PathToMDB { get { return pathToMDB; } set { pathToMDB = value; } }

        private static string pathToACCDB;
        public static string PathToACCDB { get { return pathToACCDB; } set { pathToACCDB = value; } }

        private static string _provider = "Microsoft.Jet.OLEDB.4.0";
        public static string Provider
        {
            get { return _provider ?? "Microsoft.Jet.OLEDB.4.0"; }
            set { _provider = value; }
        }

        private static string _connectionString;

        public static string ConnectionString
        {
            get
            {
                if (String.IsNullOrEmpty(_connectionString))
                {
                    if (String.IsNullOrEmpty(pathToMDB) && String.IsNullOrEmpty(pathToACCDB))
                        return null;

                    if (!String.IsNullOrEmpty(pathToMDB))
                        return $@"Provider=Microsoft.Jet.OLEDB.4.0; Data source= {pathToMDB}";
                    if (!String.IsNullOrEmpty(pathToACCDB))
                        return $@"Provider=Microsoft.Jet.OLEDB.4.0; Data source= {pathToACCDB}";
                    return null;
                }
                return _connectionString;
            }
        }


        public AccessDB()
        {

        }

        public static void BaseInitialize(string connectionString)
        {
            try
            {
                using (var connection = new OleDbConnection(connectionString))
                {
                    CreateBaseTable(connection);
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }

        /// <summary>
        /// NOT USE
        /// </summary>
        /// <param name="connection"></param>
        private static void CreateBaseTable(OleDbConnection connection)
        {
            try
            {
                DeleteBaseTable(ref connection);
                string commandString = "CREATE TABLE ExchED (InnerID, MessageType, EnvelopeID, CompanySet_key_id, DocumentID, DocName, DocNum, DocCode, );";
                using (var dbCommand = new OleDbCommand(commandString, connection))
                {
                    dbCommand.ExecuteNonQuery();
                }

            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }


        }


        public void ExecuteToDB(string[] args, int Company_key_id)
        {
            var insertCommand = $"INSERT INTO ExchED" +
                "(InnerID, MessageType, EnvelopeID, CompanySet_key_id, DocumentID, DocName, DocNum, DocCode, ArchFileName) " +
                $"VALUES ('{args[0]}', 'CMN.00202', '{args[1]}', {Company_key_id}, '{args[2]}', '{args[3]}', '{args[4]}', " +
                $"'{args[5]}', '{args[6]}')";
            try
            {
                using (var connection = new OleDbConnection(ConnectionString))
                {
                    connection.Open();
                    using (var command = new OleDbCommand(insertCommand, connection))
                        command.ExecuteNonQuery();

                    connection.Close();
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }
        private static void DeleteBaseTable(ref OleDbConnection connection)
        {
            try
            {
                string commandString = "";
                using (var dbCommand = new OleDbCommand(commandString, connection))
                {

                }
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }

        /// <summary>
        /// Access test connection with ACE Provider
        /// </summary>
        /// <param name="connectionString"></param>
        public static void TestConnectToAccessWithAce(string connectionString)
        {
            try
            {
                String connection = $@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={connectionString};Persist Security Info=True";
                using (OleDbConnection conn = new OleDbConnection(connection))
                {
                    conn.Open();

                    if (conn.State == ConnectionState.Open)
                    {
                        Console.WriteLine($"Test connection => Open\nProvider={conn.Provider}");
                        conn.Close();
                    }

                    else
                        Console.WriteLine("Test connection => ERROR");
                }
            }
            catch (Exception ex) { Console.WriteLine($"Failed to connect to data source\n{ex.Message}"); }
        }

        /// <summary>
        /// Access test connection with Jet Provider
        /// </summary>
        /// <param name="connectionString"></param>
        public static void TestConnectToAccessWithJet(string connectionString)
        {
            try
            {
                String connection = $@"Provider=Microsoft.Jet.OLEDB.4.0; Data source= {connectionString}";
                using (OleDbConnection conn = new OleDbConnection(connection))
                {
                    conn.Open();
                    if (conn.State == ConnectionState.Open)
                    {
                        Console.WriteLine($"Test connection => Open\nProvider={conn.Provider}");
                        conn.Close();
                    }
                    else
                        Console.WriteLine("Test connection => ERROR");
                }
            }
            catch (Exception ex) { Console.WriteLine($"Failed to connect to data source\n{ex.Message}"); }
        }
    }

    public enum Privider : int { }
}