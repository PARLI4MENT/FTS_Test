#define TEST

using System;

namespace XMLSigner
{
    internal class Program
    {
        static void Main(string[] args)
        {
            {
                /// Хэширование
                //string strs = "<n1:KeyInfo xmlns:n1=\"http://www.w3.org/2000/09/xmldsig#\" Id=\"KeyInfo\"><n1:X509Data><n1:X509Certificate></n1:X509Certificate></n1:X509Data></n1:KeyInfo>";
                //Console.WriteLine(SignXMLGost.HashGostR3411_2012_256(strs));

                Console.WriteLine();
            }

            {

                //XmlNs.ImplementateToXml.ImplementLinear("C:\\_test\\rawFiles\\0be68d4a-444d-4abb-a09f-ce07c9256e30\\files\\05fcc4ca-cfc1-4b59-a67c-d9a1c909b4cb\\xml\\1f2aa4ac-e439-45f6-b4ce-0a21b4f9fcb9.FreeBinaryDoc.xml");
                //SignXMLGost.SignedCmsXml(SignXMLGost.Certificate);

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


                //// Test to Access DB
                //{
                //    //'Unrecognized database format 'C:\testACCDB.accdb'.' 
                //    //AccessDB.ConnectToAccessWithAce(AccessDB.PathToACCDB);
                //}

            }
            
            Console.WriteLine("\nPress any key...");
            Console.ReadKey();
        }

    }
}
