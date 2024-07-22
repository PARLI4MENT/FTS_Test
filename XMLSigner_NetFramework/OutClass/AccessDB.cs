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

        private static string _connectionString;
        public static string ConnectionString
        {
            get
            {
                if (!String.IsNullOrEmpty(_connectionString))
                    return _connectionString;
                return null;
            }
            set { _connectionString = value; }
        }


        public AccessDB()
        {
                
        }

        public void BaseInitialize()
        {

        }
        private void CreateBaseTable()
        { 
            
        }
        private void DeleteBaseTable()
        {
            
        }

        /// <summary>
        /// Access test connection with ACE Provider
        /// </summary>
        /// <param name="connectionString"></param>
        public static void ConnectToAccessWithAce(string connectionString)
        {
            try
            {
                String connection = $@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={connectionString};Persist Security Info=True";
                using (OleDbConnection conn = new OleDbConnection(connection))
                {
                    conn.Open();

                    if (conn.State == ConnectionState.Open)
                        Console.WriteLine($"Test connection => Open\nProvider={conn.Provider}");

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
        public static void ConnectToAccessWithJet(string connectionString)
        {
            try
            {
                String connection = $@"Provider=Microsoft.Jet.OLEDB.4.0; Data source= {connectionString}";
                using (OleDbConnection conn = new OleDbConnection(connection))
                {
                    conn.Open();
                    if (conn.State == ConnectionState.Open)
                        Console.WriteLine($"Test connection => Open\nProvider={conn.Provider}");
                    else
                        Console.WriteLine("Test connection => ERROR");
                }
            }
            catch (Exception ex) { Console.WriteLine($"Failed to connect to data source\n{ex.Message}"); }
        }
    }

    public enum Privider : int
    {
    }
}
