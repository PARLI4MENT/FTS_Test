using System;
using System.IO;

namespace OutClass
{
    public static class ChangeLogger
    {
        public static void Log(string message, LogOperation logOperation)
        {
            try
            {
                File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "logs.log", $"{DateTime.Now} {logOperation} - {String.Format("{0} {1}", message, Environment.NewLine)}");
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); return; } 
        }

        public enum LogOperation
        {
            CHANGED = 1,
            CREATED = 2,
            DELETED = 3,
            RENAMED = 4,
            ERROR = 5,
        }
    }
}
