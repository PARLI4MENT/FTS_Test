using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FilesWatcher
{
    internal class Program
    {
        static void Main()
        {
            var sw = new Stopwatch();
            sw.Start();
            //XmlFTS.OutClass.ArchiveWorker.ExtractArchive("C:\\Dekl\\SEND DATA\\0a742265-fc35-4517-ad52-2e88cfa1a1d0\\0c289cc4-b89e-4ce5-9f38-b0a17b7380f8.zip");
            XMLSigner.SignXmlGost.FindGostCurrentCertificate("");
            sw.Stop();
            Console.WriteLine(sw.ElapsedTicks);
            Console.WriteLine();
            Console.ReadKey();
        }

        //static void Main()
        //{
        //    var watcher = new FileSystemWatcher(@"C:\_1");

        //    watcher.NotifyFilter = NotifyFilters.Attributes
        //                         | NotifyFilters.CreationTime
        //                         | NotifyFilters.DirectoryName
        //                         | NotifyFilters.FileName
        //                         | NotifyFilters.LastAccess
        //                         | NotifyFilters.LastWrite
        //                         | NotifyFilters.Security
        //                         | NotifyFilters.Size;

        //    watcher.Changed += OnChanged;
        //    watcher.Created += OnCreated;
        //    watcher.Deleted += OnDeleted;
        //    watcher.Renamed += OnRenamed;
        //    watcher.Error += OnError;

        //    watcher.Filter = "*.xml";
        //    watcher.IncludeSubdirectories = true;
        //    watcher.EnableRaisingEvents = true;

        //    Console.WriteLine("Press enter to exit.");
        //    Console.ReadLine();
        //}

        private static void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }
            Console.WriteLine($"Changed: {e.FullPath}");
        }

        private static void OnCreated(object sender, FileSystemEventArgs e)
        {
            string value = $"Created: {e.FullPath}";
            Console.WriteLine(value);
        }

        private static void OnDeleted(object sender, FileSystemEventArgs e) =>
            Console.WriteLine($"Deleted: {e.FullPath}");

        private static void OnRenamed(object sender, RenamedEventArgs e)
        {
            Console.WriteLine($"Renamed:");
            Console.WriteLine($"    Old: {e.OldFullPath}");
            Console.WriteLine($"    New: {e.FullPath}");
        }

        private static void OnError(object sender, ErrorEventArgs e) =>
            PrintException(e.GetException());

        private static void PrintException(Exception ex)
        {
            if (ex != null)
            {
                Console.WriteLine($"Message: {ex.Message}");
                Console.WriteLine("Stacktrace:");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine();
                PrintException(ex.InnerException);
            }
        }
    }
}
