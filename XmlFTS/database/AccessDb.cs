using System;
using System.Data.OleDb;
using System.Data;
using System.Diagnostics;
using System.Configuration;
using System.IO;
using System.Runtime.InteropServices;

namespace SQLNs
{
    public class AccessDB : IDisposable
    {
        private static string _pathToMDB = null;

        private static string _pathToACCDB = null;

        private static string _connectionString;

        /// <summary>
        /// Функция проверка строки подключения MS Access. Возвращает null если ни один из способов не действителен
        /// </summary>
        public static string ConnectionString
        {
            get
            {
                if (String.IsNullOrEmpty(_connectionString))
                {
                    if (String.IsNullOrEmpty(_pathToMDB) && String.IsNullOrEmpty(_pathToACCDB))
                        return null;

                    if (!String.IsNullOrEmpty(_pathToMDB))
                        return $@"Provider=Microsoft.Jet.OLEDB.4.0; Data source= {_pathToMDB}";
                    if (!String.IsNullOrEmpty(_pathToACCDB))
                        return $@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={_pathToACCDB};Persist Security Info=False;";
                    return null;
                }
                return _connectionString;
            }
        }

        private static OleDbConnection _oleDbConnection;

        /// <summary> Конструктор MS Access по-умолчанию </summary>
        public AccessDB() { }

        /// <summary> Конструктор MS Access</summary>
        /// <param name="pathToDb"> Путь к файлу MS Access </param>
        public AccessDB(string pathToDb)
        {
            switch (Path.GetExtension(pathToDb).ToLower())
            {
                case ".accdb":
                    _pathToACCDB = pathToDb;
                    _connectionString = $@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={pathToDb};Persist Security Info=False;";
                    break;
                case ".mdb":
                    _pathToMDB = pathToDb;
                    _connectionString = $@"Provider=Microsoft.Jet.OLEDB.4.0; Data source= {pathToDb};";
                    break;
                default:
                    Debug.WriteLine("Extension not meet requirements MS Access!");
                    break;
            }

            _oleDbConnection = new OleDbConnection(ConnectionString);
            _oleDbConnection.Open();
        }

        /// <summary> Выполнение базовой инициализации MS Access (не доделано)</summary>
        /// <param name="connectionString"> Строка подключения </param>
        public static void BaseInitialize(string connectionString)
        {
            try
            {
                using (var connection = new OleDbConnection(connectionString))
                    CreateBaseTable(connection);
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }

        /// <summary> NOT USE </summary>
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

        /// <summary> Выполнение запроса к БД</summary>
        /// <param name="args"> Массив строк со значениями </param>
        /// <param name="Company_key_id">Код компании (по-умолчанию 1) </param>
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
                    if (ConnectionString is null)
                        return;

                    connection.Open();
                    using (var command = new OleDbCommand(insertCommand, connection))
                        command.ExecuteNonQuery();
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }

        /// <summary> Выполнение запроса к БД</summary>
        /// <param name="args"> Массив строк со значениями </param>
        /// <param name="Company_key_id">Код компании (по-умолчанию 1) </param>
        public void ExecuteToDB(string[] args)
        {
            var insertCommand = $"INSERT INTO ExchED" +
                "(InnerID, MessageType, EnvelopeID, CompanySet_key_id, DocumentID, DocName, DocNum, DocCode, ArchFileName) " +
                $"VALUES ('{args[0]}', 'CMN.00202', '{args[1]}', 1, '{args[2]}', '{args[3]}', '{args[4]}', " +
                $"'{args[5]}', '{args[6]}')";
            try
            {
                using (var connection = new OleDbConnection(ConnectionString))
                {
                    if (ConnectionString is null)
                        return;

                    connection.Open();
                    using (var command = new OleDbCommand(insertCommand, connection))
                        command.ExecuteNonQuery();
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }

        /// <summary> Выполнить запрос </summary>
        /// <param name="args"> Массив данных типа string </param>
        public void Execute(string[] args)
        {
            var insertCommand = $"INSERT INTO ExchED" +
                "(InnerID, MessageType, EnvelopeID, CompanySet_key_id, DocumentID, DocName, DocNum, DocCode, ArchFileName) " +
                $"VALUES ('{args[0]}', 'CMN.00202', '{args[1]}', 1, '{args[2]}', '{args[3]}', '{args[4]}', " +
                $"'{args[5]}', '{args[6]}')";

            try
            {
                using (var command = new OleDbCommand(insertCommand, _oleDbConnection))
                    command.ExecuteNonQuery();
            }
            catch (Exception ex) { Debug.WriteLine(ex.Message); return; }
        }

        /// <summary> Тестовый запрос на добавление и удаление данных в таблицу </summary>
        /// <returns></returns>
        public static bool TestExecuteToDB()
        {
            var insertCommand = "INSERT INTO ExchED" +
                "(InnerID, MessageType, EnvelopeID, CompanySet_key_id, DocumentID, DocName, DocNum, DocCode, ArchFileName) " +
                $"VALUES ('Test_InnerID', 'Test_CMN.00202', 'Test_EnvelopeID', 0, 'Test_DocumentID', 'Test_DocName', 'Test_DocNum', " +
                $"'Test_DocCode', 'Test_ArchFileName')";
            var deleteCommand = "DELETE * FROM ExchED WHERE EnvelopeID = 'Test_EnvelopeID';";

            try
            {
                using (var connection = new OleDbConnection(ConnectionString))
                {
                    if (ConnectionString is null)
                        return false;

                    connection.Open();
                    using (var command = new OleDbCommand(insertCommand, connection))
                        command.ExecuteNonQuery();
                    Debug.WriteLine("Insert testing is DONE!");

                    using (var command = new OleDbCommand(deleteCommand, connection))
                        command.ExecuteNonQuery();
                    Debug.WriteLine("Delete testing is DONE!");

                    connection.Close();
                    return true;
                }
            }
            catch (Exception ex) { Debug.WriteLine(ex.Message); return false; }
        }

        /// <summary> Полная очистка данный в БД </summary>
        /// <param name="tableName"> Имя таблицы </param>
        public static void DeleteDataTable(string tableName = "ExchED")
        {
            var clearData = $"DELETE * FROM {tableName}";

            try
            {
                using (var connection = new OleDbConnection(ConnectionString))
                {
                    if (ConnectionString is null) { Debug.WriteLine("Connection string is Null."); return; }

                    connection.Open();
                    using (var command = new OleDbCommand(clearData, connection))
                        command.ExecuteNonQuery();

                    connection.Close();
                    return;
                }
            }
            catch (Exception ex) { Debug.WriteLine(ex.Message); return; }
        }

        /// <summary> Удаление всех данных из всех таблиц. Структура таблиц и сами таблицы остаються. </summary>
        public static void DeleteAllDataFromTables()
        {
            string query = "SELECT MSysObjects.Name AS table_name FROM MSysObjects WHERE (((Left([Name],1))<>\"~\") " +
                "AND ((Left([Name],4))<>\"MSys\") AND ((MSysObjects.Type) In (1,4,6))) ORDER BY MSysObjects.Name";
            string[] tablesName;

            try
            {
                using (var connection = new OleDbConnection(ConnectionString))
                {
                    if (ConnectionString is null) { Debug.WriteLine("Connection string is Null."); return; }

                    connection.Open();
                    using (var command = new OleDbCommand(query, connection))
                        command.ExecuteNonQuery();

                    connection.Close();
                    return;
                }
            }
            catch (Exception ex) { Debug.WriteLine(ex.Message); return; }
        }

        /// <summary> Не доделано </summary>
        /// <param name="connection"></param>
        private static void DeleteBaseTable(ref OleDbConnection connection)
        {
            try
            {
                string commandString = "";
                using (var dbCommand = new OleDbCommand(commandString, connection)) { }
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }

        /// <summary> Access test connection with ACE Provider </summary>
        /// <param name="connectionString">Только для *.accdb файлов </param>
        public static void TestConnectToAccessWithAce(string pathToAccdb)
        {
            try
            {
                String connection = $@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={pathToAccdb};Persist Security Info=True";
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

        /// <summary> Access test connection with Jet Provider </summary>
        /// <param name="connectionString"> Только *.mdb файлы </param>
        public static void TestConnectToAccessWithJet(string pathToMdb)
        {
            try
            {
                String connection = $@"Provider=Microsoft.Jet.OLEDB.4.0; Data source= {pathToMdb}";
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


        public void Dispose()
        {
            if (_oleDbConnection.State == ConnectionState.Open)
                _oleDbConnection.Close();
        }
    }
}