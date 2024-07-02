using PostgresSQL_Test.Other;
using SQLTest;

namespace MainNs
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //PgSqlConnect();
            //PgSqlCreateDatabase();
            //PgSqlCreateTable();

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

            /// Don`t use
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

            //PgClearData("testTabel1");
            //PgInsertData("testTabel1", 100000);
            //PgInsertDataParallel("testTabel1", 100000);

            #region Test PG Class
            //{
            //    var pgSql = new PgSql();
            //    pgSql.PgInsertData();
            //    pgSql.PgInsertDataParallel(100);
            //}
            #endregion


            {
                string str = "C:\\_DestinationFolder\\0be68d4a-444d-4abb-a09f-ce07c9256e30.1f2aa4ac-e439-45f6-b4ce-0a21b4f9fcb9.FreeBinaryDoc.xml";

                string fileName, Id;
                ExtractXML.ExtractId("", out Id, out fileName);
                Console.WriteLine($"ID => {Id}\tPath => {fileName}");
            }

            Console.ReadKey();
        }

    }
};