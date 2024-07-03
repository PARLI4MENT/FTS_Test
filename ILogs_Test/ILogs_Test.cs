using System;
using System.Xml;
using ILogger;

namespace MainNs
{

    public class ILogs_Test
    {
        public static void Main(string[] args)
        {
            ConsoleLogger.PathToLogFile = $"{Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Arch_docs.log")}";
            ConsoleLogger.OpenLogFile = false;

            int index = 0;
            while (index < 100)
            {
                ConsoleLogger.Log(LogLevel.Info, $"TestMsg {index}", true);
                //Thread.Sleep(1000);
                index++;
            }

            Console.ReadLine();
        }
    }
}