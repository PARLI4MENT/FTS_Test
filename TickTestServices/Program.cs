using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace TickTestServices
{
    internal class Program
    {
        private static System.Timers.Timer timer;
        private static DateTime lastWrite;

        static void Main(string[] args)
        {
            var task1 = Task.Run(() =>
            {
                Console.WriteLine("task1 => started");
                timer = new System.Timers.Timer();
                timer.Interval = 1000;
                timer.Elapsed += new System.Timers.ElapsedEventHandler(Tick);
                timer.Start();
            });

            Console.ReadKey();
        }

        private static void Tick(object sender, System.Timers.ElapsedEventArgs e)
        {
            System.IO.FileInfo info = new System.IO.FileInfo("C:\\_1");

            if (lastWrite == DateTime.MinValue)
            {
                lastWrite = info.LastWriteTime;
            }

            if (lastWrite.CompareTo(info.LastWriteTime) == -1)
            {
                if (Directory.GetFiles(info.FullName).Count() > 0)
                {
                    timer.Stop();
                    foreach (string file in Directory.GetFiles(info.FullName))
                    {
                        Console.WriteLine($"\n{file}");
                        string combineMovePathFile = Path.Combine("C:\\_3", Path.GetFileName(file));
                        File.Move(file, combineMovePathFile);
                        Console.WriteLine($"\n{combineMovePathFile}");
                    }

                    timer.Start();
                }
            }
        }
    }
}
