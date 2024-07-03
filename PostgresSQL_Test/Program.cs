using SQLTest;

namespace MainNs
{
    public class Program
    {
        public static void Main(string[] args)
        {
            /// Dont use
            #region
            //{
            //    var sw = new Stopwatch();
            //    sw.Restart();

            //    var thread = new Thread(() =>
            //    {
            //        PgInsertData("testTabel1", 0, 10000);
            //    });

            //    thread.Start();
            //    thread.Join();

            //    sw.Stop();
            //    Console.WriteLine(sw.Elapsed);
            //}
            #endregion

            /// Dont use
            #region
            //{
            //    var sw = new Stopwatch();
            //    sw.Start();
            //    sw.Restart();

            //    Task th = Task.Factory.StartNew(() => PgInsertData("testTabel1", 10000));
            //    Task.WhenAll(th).ContinueWith(task => sw.Stop());

            //    Console.WriteLine(sw.Elapsed);

            //}
            #endregion

            string pathTemplate = "C:\\_test\\create_doc_in_arch.xml";
            string pathOut = "C:\\_test\\OUT\\";
            string pathToArch = "C:\\_test\\arch";

            SqlTest.RenamerXML.RootDir = "C:\\_test";
            SqlTest.RenamerXML.ParseFileByMaskedParallel();

            Console.ReadKey();
        }
    }
};