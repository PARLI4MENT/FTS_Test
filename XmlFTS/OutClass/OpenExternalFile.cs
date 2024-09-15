using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlFTS.OutClass
{
    public static class OpenExternalFile
    {
        public static void Open(string pathToFile)
        {
            try
            {
                if (File.Exists(pathToFile))
                {
                    ProcessStartInfo processStartInfo = new ProcessStartInfo
                    {
                        Arguments = Path.GetDirectoryName(pathToFile),
                        FileName = Path.GetFileName(pathToFile)
                    };
                    Process.Start(processStartInfo);
                }
            }
            catch (Exception ex) { Debug.WriteLine(ex.Message); }
            
        }
    }
}
