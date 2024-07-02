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


            var pgSql = new PgSql();
            PgSql.PgSqlCheckConnection();

            Console.ReadKey();
        }

    }
};