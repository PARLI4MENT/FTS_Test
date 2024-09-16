#define TEST
///5.23.0/3.4.16

using System;
using XmlFTS;
using XmlFTS.OutClass;

namespace XMLSigner
{
    public static class Program
    {       
        public static void Main(string[] args)
        {

            ProcessXML.StartProcess();

            Console.Write("\nPress any key...");
            Console.ReadKey();
        }
    }
}