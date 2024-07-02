using System;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;

namespace MainNs
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var rgx = new Regex(@"(\w).filesList.xml");
            //MatchCollection matchCollection = rgx.Match();

            var tmp = FileList();

            Console.ReadKey();
        }

        private static List<string> FileList()
        {
            string pathToInputFolder = "C:\\_DestinationFolder";
            var lists = new List<string>();

#if DEBUG
            var sw = new Stopwatch();
            sw.Start();
#endif
            string[] listsFile = Directory.GetFiles(pathToInputFolder);

            var regex = new Regex(@"filesList.xml", RegexOptions.IgnoreCase);

//#if DEBUG
//            int index = 0;
//#endif

            foreach ( string listFile in listsFile )
            {
                //Console.WriteLine(Path.GetFileName(listFile));
                MatchCollection collect = regex.Matches(Path.GetFileName(listFile));
                if (collect.Count == 0)
                {
#if DEBUG
                    Console.WriteLine(Path.GetFileName(listFile));
#endif
                    lists.Add(listFile);


                    /// Add per regular expression
//                    foreach (Match lst in collect)
//                    {
//#if DEBUG
//                        if (lst.Value != "filesList.xml")
//                        {

//                            Console.WriteLine($"[{index}] => {lst.Value}\n\t# => {Path.GetFileName(listFile)}");
//                            index++;
//                        }
//#endif
//                    }
                }
            }


#if DEBUG
            sw.Stop();
            Console.WriteLine(@$"Lead time (AddRange) is: {sw.Elapsed}");
            Console.WriteLine($@"Count => {lists.Count}");
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
            return lists;
        }
    }
}