
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.IO;
/// Сделать Запись в файл
namespace ILogger
{
    public interface ILogger
    {
        /// <summary>
        /// Main method for write to log file
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="message"></param>
        /// <param name="writeToLogFile"></param>
        abstract static void Log(LogLevel logLevel, string message, bool writeToLogFile);

        private static string _pathToLogFile;
        public static abstract string PathToLogFile { get; set; }

        private static bool _openLogFile;
        public static bool OpenLogFile { private get; set; }

        private static bool _writeToLogFile;
        public static bool WriteToLogFile { private get; set; } = true;
    }

    /// <summary>
    /// Class for logging info to Console or/and Log file
    /// </summary>
    public class ConsoleLogger : ILogger
    {
        private static bool _openLogFile = false;
        public static bool OpenLogFile
        { 
            private get { return _openLogFile; }
            set { _openLogFile = value; }
        }

        private static bool _writeToLogFile = true;
        public static bool WriteToLogFile
        {
            private get { return _writeToLogFile; }
            set { _writeToLogFile = value; }
        }

        private static string _pathToLogFile = $@"{Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Arch_docs.log")}";
        public static string PathToLogFile
        {
            get { return _pathToLogFile; }
            set { _pathToLogFile = Path.GetFullPath(value); }
        }

        public static async void Log(LogLevel logLevel, string message, bool writeToLogFile = true)
        {
            switch (logLevel)
            {
                case LogLevel.Debug:
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"[Debug] : {DateTime.Now} => {message}");
                    if (writeToLogFile)
                        File.WriteAllTextAsync(PathToLogFile, $"[Debug] : {DateTime.Now} => {message}");
                    if (OpenLogFile)
                        File.Open(PathToLogFile, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
                    break;

                case LogLevel.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"[Info] : {DateTime.Now} => {message}");
                    if (writeToLogFile)
                        //await File.WriteAllTextAsync(PathToLogFile, $"[Info] : {DateTime.Now} => {message}");
                        await using(StreamWriter sw = File.AppendText(PathToLogFile))
                            await sw.WriteLineAsync($"[Info] : {DateTime.Now} => {message}");

                    //if (OpenLogFile)
                    //    File.Open(PathToLogFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
                    break;
                case LogLevel.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"[Warning] : {DateTime.Now} => {message}");
                    if (writeToLogFile) 
                        File.WriteAllTextAsync( PathToLogFile, $"[Warning] : {DateTime.Now} => {message}");
                    if (OpenLogFile)
                        File.Open(PathToLogFile, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read);
                    break;
                
                case LogLevel.Error:
                
                case LogLevel.Fatal:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[Fatal] : {DateTime.Now} => {message}");
                    if( writeToLogFile )
                        File.WriteAllTextAsync( PathToLogFile, $"[Warning] : {DateTime.Now} => {message}");
                    if (OpenLogFile)
                        File.Open(PathToLogFile, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
                    break;

            }
            Console.ResetColor();
        }

        private static async Task AppendLineToFile([NotNull] string path, string line)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentOutOfRangeException(nameof(path), path, "Was null or whitepsace.");

            if (!File.Exists(path))
                throw new FileNotFoundException("File not found.", nameof(path));

            using (var file = File.Open(path, FileMode.Append, FileAccess.Write))
            using (var writer = new StreamWriter(file))
            {
                await writer.WriteLineAsync(line);
                await writer.FlushAsync();
            }
        }
    }
    public enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error,
        Fatal
    }
}