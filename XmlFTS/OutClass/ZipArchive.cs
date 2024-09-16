using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;

namespace XmlFTS
{
    public static class ArchiveWorker
    {
        /// <summary>Извлекает из ZIP-архива XML-файлы в переименнованном виде</summary>
        /// <param name="pathToZip"></param>
        /// <param name="dirDestination">Папка назначения</param>
        /// <remarks> Папка назначения содержит папку "code" </remarks>
        public static List<string> ExtractZipArchive(string pathToZip, string dirDestination = "C:\\_2\\ExtractionFiles")
        {
            if (File.Exists(pathToZip))
            {
                string code = Path.GetFileName(Path.GetDirectoryName(pathToZip));
                var listXmlPaths = new List<string>();

                if (!Directory.Exists(Path.Combine(dirDestination)))
                    Directory.CreateDirectory(Path.Combine(dirDestination));

                using (ZipArchive zipArch = new ZipArchive(File.OpenRead(pathToZip)))
                {
                    foreach (ZipArchiveEntry entry in zipArch.Entries)
                    {
                        if (entry.FullName.Contains("xml"))
                        {
                            string pathDest = Path.Combine(dirDestination, string.Concat(code, ".", entry.Name));
                            entry.ExtractToFile(pathDest, true);
                            listXmlPaths.Add(pathDest);
                        }
                    }
                }
                return listXmlPaths;
            }
            return null;
        }

        /// <summary>Извлекает из ZIP-архива XML-файлы в переименнованном виде</summary>
        /// <param name="pathToZip">Путь к папке с архивами</param>
        public static void ExtractZipArchive(string pathToZip, int Count)
        {
            if (File.Exists(pathToZip))
            {
                string code = Path.GetFileName(Path.GetDirectoryName(pathToZip));
                int count = 0;

                if (!Directory.Exists(Path.Combine("C:\\_2\\ExtractionFiles")))
                    Directory.CreateDirectory(Path.Combine("C:\\_2\\ExtractionFiles"));

                using (ZipArchive zipArch = new ZipArchive(File.OpenRead(pathToZip)))
                {
                    foreach (ZipArchiveEntry entry in zipArch.Entries)
                    {
                        if (entry.FullName.Contains("xml"))
                        {
                            string pathDest = Path.Combine("C:\\_2\\ExtractionFiles", string.Concat(code, ".", entry.Name));
                            entry.ExtractToFile(pathDest, true);
                        }
                    }
                }
            }
        }
    }
}
