using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using XmlFTS.OutClass;

namespace FileNs
{
    public class ProcessXML
    {
        private ProcessXML() { }

        public static void StartProcess(bool TimerIsEnable = false, bool LogsIsEnable = false)
        {
            while (true)
            {
                var sw = new Stopwatch();
                sw.Start();
                int SummaryFiles = 0;
                Console.WriteLine("Start...");

                var rawSrcFolders = Directory.GetDirectories("C:\\Dekl\\SEND DATA");
                foreach (var rawSrcFolder in rawSrcFolders)
                {
                    /// #1 Extraction ZIP
                    ArchiveWorker.ExtractZipArchive(Directory.GetFiles(rawSrcFolder, "*.zip")[0]);

                    /// #2 Rename Copy
                    string[] xmlFiles = Directory.GetFiles(rawSrcFolder, "*.xml");
                    // RenameMoveFileOnly(xmlFiles);
                }

                Console.WriteLine("Sort start");

                ///// #3 Sort
                string[] notSortedFiles = Directory.GetFiles("C:\\_2\\ExtractionFiles", "*.xml");
                // SortXml(notSortedFiles);

                SummaryFiles += Directory.GetFiles("C:\\_2\\ExtractionFiles", "*.xml").Count();

                sw.Stop();
                Console.WriteLine();
                Console.WriteLine($"General => {SummaryFiles} count || {sw.ElapsedMilliseconds / 1000} sec.");
                //Console.WriteLine($"AVG => {SummaryFiles / (sw.ElapsedMilliseconds / 1000)} sec.");

            }
            Debug.WriteLine("Process stoped");
            Debug.WriteLine("Press any key...");
            return;
        }
    }
}
