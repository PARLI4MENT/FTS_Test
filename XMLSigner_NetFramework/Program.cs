#define TEST

using GostCryptography.Xml;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;
using System.Xml;

namespace XMLSigner
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start process...");

            /// Rename and move to intermidateFiles XML files
            FileNs.RenamerXML.RenameMoveParallel("C:\\_test\\rawFiles");

            var sw = new Stopwatch();
            var swTotal = new Stopwatch();
            sw.Start();
            swTotal.Start();

            // Inplement to XML, signing and sending request to BD
            Console.WriteLine("\nStart implement...");
            XmlNs.ImplementateToXml.ImplementParallel(Directory.GetFiles("C:\\_test\\intermidateFiles"));
            sw.Stop();
            Console.WriteLine($"Time implement => {sw.ElapsedMilliseconds / 1000},{sw.ElapsedMilliseconds % 1000} sec");
            Console.WriteLine($"Total destination files => {Directory.GetFiles("C:\\_test\\implementFiles").Count()} units");
            Console.WriteLine($"AVG => {Directory.GetFiles("C:\\_test\\intermidateFiles").Count() / ((int)sw.ElapsedMilliseconds / 1000)}");

            sw.Restart();
            Console.WriteLine("\nStart signing XML...");
            SignXMLGost.SignXmlFiles(Directory.GetFiles("C:\\_test\\implementFiles"), SignXMLGost.cert);
            sw.Stop();
            swTotal.Stop();
            Console.WriteLine($"Time signed => {sw.ElapsedMilliseconds / 1000},{sw.ElapsedMilliseconds % 1000} sec");
            Console.WriteLine($"Total destination files => {Directory.GetFiles("C:\\_test\\signedFiles").Count()} units");
            Console.WriteLine($"AVG => {(Directory.GetFiles("C:\\_test\\implementFiles").Count()) / ((int)sw.ElapsedMilliseconds / 1000)}");

            Console.WriteLine($"\nTotal time => {swTotal.ElapsedMilliseconds / 1000},{swTotal.ElapsedMilliseconds % 1000} sec");
            Console.WriteLine("DONE !");

            Console.WriteLine("\nPress any key...");
            Console.ReadKey();
        }

    }
}
