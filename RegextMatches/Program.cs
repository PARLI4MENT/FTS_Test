using System.Diagnostics;
using System.Text.RegularExpressions;

namespace RegexMatchesNs
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var tmp = FileList();

            Console.ReadKey();
        }

        public static List<string> FileList()
        {
            string pathToInputFolder = "C:\\_DestinationFolder";
            var nonFilesList = new List<string>();
            var filesLists = new List<string>();

#if DEBUG
            var sw = new Stopwatch();
            sw.Start();
#endif
            string[] listsFile = Directory.GetFiles(pathToInputFolder);

            var regex = new Regex(@"filesList", RegexOptions.IgnoreCase);

            foreach ( string listFile in listsFile )
            {
                MatchCollection collect = regex.Matches(Path.GetFileName(listFile));
                // Для не "*filesList.xml"
                if (collect.Count == 0)
                    nonFilesList.Add(listFile);
                if (collect.Count > 0)
                    filesLists.Add(listFile);
            }

#if DEBUG
            sw.Stop();
            Console.WriteLine($"\nLead time (AddRange) is: {sw.Elapsed}");
            Console.WriteLine($"\tFiles list => {filesLists.Count}");
            Console.WriteLine($"\tNon files list => {nonFilesList.Count}");
            Console.WriteLine($"\tTotal => {listsFile.Length}");
            Console.WriteLine();
#endif

#if DEBUG
            //int indx = 0;
            //foreach (var file in lists)
            //{
            //    Console.WriteLine($"[{indx}] => {file}");
            //    indx++;
            //}
#endif
            return nonFilesList;
        }
    }
}