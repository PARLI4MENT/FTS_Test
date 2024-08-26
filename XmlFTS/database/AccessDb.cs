#define DEBUG

using System;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
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
#if DEBUG
                    Debug.WriteLine("Отсутствует путь к файлу MS Access");
#endif
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
                _connectionString = $"Provider=Microsoft.Jet.OLEDB.4.0; Data source= {value};";
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

        /// <summary> NOT USABLE </summary>
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
        /// <param name="Company_key_id">Код компании (по-умолчанию = 1) </param>
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
                    using (var command = new OleDbCommand(insertCommand, _oleDbConnection))
                        command.ExecuteNonQuery();
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }

        /// <summary> Выполнить запрос </summary>
        /// <param name="args"> Массив данных типа string </param>
        public void ExecuteToDB(string[] args)
        {
            var insertCommand = $"INSERT INTO ExchED" +
                "(InnerID, MessageType, EnvelopeID, CompanySet_key_id, DocumentID, DocName, DocNum, DocCode, ArchFileName) " +
                $"VALUES ('{args[0]}', 'CMN.00202', '{args[1]}', 1, '{args[2]}', '{args[3]}', '{args[4]}', " +
                $"'{args[5]}', '{args[6]}')";

            try
            {
                if (ConnectionString is null)
                    return;

                using (var command = new OleDbCommand(insertCommand, _oleDbConnection))
                    command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine(ex.Message);
#endif
                return;
            }
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
#if DEBUG
                    Debug.WriteLine("Insert testing is DONE!");
#endif

                    using (var command = new OleDbCommand(deleteCommand, connection))
                        command.ExecuteNonQuery();
#if DEBUG
                    Debug.WriteLine("Delete testing is DONE!");
#endif
                    connection.Close();
                    return true;
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine(ex.Message);
#endif
                return false;
            }
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
                    if (ConnectionString is null)
                    {
#if DEBUG
                        Debug.WriteLine("Connection string is Null.");
#endif
                        return;
                    }

                    connection.Open();
                    using (var command = new OleDbCommand(clearData, connection))
                        command.ExecuteNonQuery();

                    connection.Close();
                    return;
                }
            }
            catch (Exception ex) 
            {
#if DEBUG
                Debug.WriteLine(ex.Message);
#endif
                return;
            }
        }

        /// <summary> Удаление всех данных из всех таблиц. Структура таблиц и сами таблицы остаються. </summary>
        private static void DeleteAllDataFromTables()
        {
            string query = "SELECT MSysObjects.Name AS table_name FROM MSysObjects WHERE (((Left([Name],1))<>\"~\") " +
                "AND ((Left([Name],4))<>\"MSys\") AND ((MSysObjects.Type) In (1,4,6))) ORDER BY MSysObjects.Name";
            string[] tablesName;

            try
            {
                using (var connection = new OleDbConnection(ConnectionString))
                {
                    if (ConnectionString is null)
                    {
#if DEBUG
                        Debug.WriteLine("Connection string is Null.");
#endif
                        return;
                    }

                    connection.Open();
                    using (var command = new OleDbCommand(query, connection))
                        command.ExecuteNonQuery();

                    connection.Close();
                    return;
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine(ex.Message);
#endif
                return;
            }
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
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine(ex.Message);
#endif
                return;
            }
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
#if DEBUG
                        Debug.WriteLine($"Test connection => Open\nProvider={conn.Provider}");
#endif
                        conn.Close();
                    }

                    else
#if DEBUG
                        Debug.WriteLine("Test connection => ERROR");
#endif
                }
            }
            catch (Exception ex) { Debug.WriteLine($"Failed to connect to data source\n{ex.Message}"); }
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
#if DEBUG
                        Debug.WriteLine($"Test connection => Open\nProvider={conn.Provider}");
#endif
                        conn.Close();
                    }
                    else
#if DEBUG
                        Debug.WriteLine("Test connection => ERROR");
#endif
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine($"Failed to connect to data source\n{ex.Message}");
#endif
            }
        }

        public void Dispose()
        {
            if (_oleDbConnection.State == ConnectionState.Open)
                _oleDbConnection.Close();
        }
    }
}