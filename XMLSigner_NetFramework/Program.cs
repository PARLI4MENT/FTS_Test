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
            StaticPathConfiguration.TemplateXML = "C:\\_2\\template.xml";
            StaticPathConfiguration.PathImplementFolder = "C:\\_2\\ImplFiles";
            ProcessXML.StartProcess();

            Console.Write("\nPress any key...");
            Console.ReadKey();
        }
    }
}