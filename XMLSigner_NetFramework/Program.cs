#define TEST

using SQLTestNs;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using static System.Net.WebRequestMethods;
using System.Security.Cryptography.X509Certificates;

namespace XMLSigner
{
    internal class Program
    {
        static void Main(string[] args)
        {
            
            Console.WriteLine("Start process...");

            //XmlNs.ImplementateToXml.ImplementLinear("C:\\_test\\rawFiles\\0be68d4a-444d-4abb-a09f-ce07c9256e30\\files\\05fcc4ca-cfc1-4b59-a67c-d9a1c909b4cb\\xml\\1f2aa4ac-e439-45f6-b4ce-0a21b4f9fcb9.FreeBinaryDoc.xml");
            SignXMLGost.SignedCmsXml(SignXMLGost.Certificate);

            /// Rename and move to intermidateFiles XML files
            //FileNs.RenamerXML.RenameMoveParallel("C:\\_test\\rawFiles");

            //AccessDB.PathToMDB = "C:\\_test\\testMDB.mdb";

            //var sw = new Stopwatch();
            //var swTotal = new Stopwatch();
            //sw.Start();
            //swTotal.Start();

            //Inplement to XML, signing and sending request to BD
            //Console.WriteLine("\nStart implement...");
            //XmlNs.ImplementateToXml.ImplementParallel(Directory.GetFiles("C:\\_test\\intermidateFiles"));
            //sw.Stop();
            //Console.WriteLine($"Time implement => {sw.ElapsedMilliseconds / 1000},{sw.ElapsedMilliseconds % 1000} sec");
            //Console.WriteLine($"Total destination files => {Directory.GetFiles("C:\\_test\\implementFiles").Count()} units");
            //Console.WriteLine($"AVG => {Directory.GetFiles("C:\\_test\\intermidateFiles").Count() / ((int)sw.ElapsedMilliseconds / 1000)}");

            //sw.Restart();
            //Console.WriteLine("\nStart signing XML...");

            //SignXMLGost.SignFullXml(Directory.GetFiles("C:\\_test\\implementFiles"), SignXMLGost.Certificate);

            //sw.Stop();
            //swTotal.Stop();
            //Console.WriteLine($"Time signed => {sw.ElapsedMilliseconds / 1000},{sw.ElapsedMilliseconds % 1000} sec");
            //Console.WriteLine($"Total destination files => {Directory.GetFiles("C:\\_test\\signedFiles").Count()} units");
            //Console.WriteLine($"AVG => {(Directory.GetFiles("C:\\_test\\implementFiles").Count()) / ((int)sw.ElapsedMilliseconds / 1000)}");

            //Console.WriteLine($"\nTotal time => {swTotal.ElapsedMilliseconds / 1000},{swTotal.ElapsedMilliseconds % 1000} sec");
            Console.WriteLine("DONE !");

            Console.WriteLine("\nPress any key...");

            //// Test to Access DB
            //{
            //    //'Unrecognized database format 'C:\testACCDB.accdb'.' 
            //    //AccessDB.ConnectToAccessWithAce(AccessDB.PathToACCDB);
            //}


            Console.ReadKey();
        }

    }
}
