using System;
using System.Diagnostics;
using System.IO;

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
