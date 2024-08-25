#define DEBUG

using System;
using System.Data.OleDb;
using System.Data;
using System.Diagnostics;
using System.Configuration;
using System.IO;
using System.Runtime.InteropServices;
using XMLSigner.SQL;
using XmlFTS.OutClass;

namespace SQLNs
{

    /// <summary> Класс для доступа к MS Access </summary>
    /// <remarks>
    /// Первая инициализация обязательна. Данные строки подключения сохраняются в файл конфигурации.
    /// </remarks>
    public class AccessDB : IDisposable
    {
        private static string connectionKey = "AccessConnectionString";
        private static string _connectionString;
        
        /// <summary> Data source for MS Access </summary>
        private static OleDbConnection _oleDbConnection;

        /// <summary> Конструктор по-умолчанию </summary>
        /// <remarks> Проверяет на наличие строки подключения к MS Access Jet Provider </remarks>
        public AccessDB()
        {
            if (string.IsNullOrEmpty(_connectionString))
            { 
                if (string.IsNullOrEmpty(Config.ReadSettings(connectionKey)))
                {
                    Debug.WriteLine("Отсутствует путь к файлу MS Access");
                    return;
                }
            }
            _connectionString = Config.ReadSettings(connectionKey);
        }

        /// <summary> Конструктор MS Access по-умолчанию </summary>
        public AccessDB(string pathToMdb)
        {
            if (File.Exists(pathToMdb))
            {
                _connectionString = $"Provider=Microsoft.Jet.OLEDB.4.0; Data source= {pathToMdb};";
                Config.AddUpdateAppSettings(connectionKey, _connectionString);
            }

            //_oleDbConnection = new OleDbConnection(ConnectionString);
            //_oleDbConnection.Open();
        }

        /// <summary> Функция проверка строки подключения MS Access. Возвращает null если ни один из способов не действителен </summary>
        public static string ConnectionString
        {
            get { return _connectionString; }
            set
            {
                _connectionString= $"Provider=Microsoft.Jet.OLEDB.4.0; Data source= {value};";
                Config.AddUpdateAppSettings(connectionKey, _connectionString);
            }
        }

        /// <summary> Выполнение базовой инициализации MS Access (не доделано)</summary>
        /// <param name="pathToMDB"> Путь к файлу .mdb </param>
        private static void BaseInitialize()
        {
            try
            {
                using (var connection = new OleDbConnection(_connectionString))
                    CreateBaseTable(connection);
            }
            catch (OleDbException ex) { Debug.WriteLine(ex.Message); return; }
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
        private static void TestConnectToAccessWithAce(string pathToAccdb)
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