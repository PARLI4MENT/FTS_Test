using System;
using System.IO;
using System.IO.Compression;

namespace XmlFTS.OutClass
{
    public static class ArchiveWorker
    {
        /// <summary> Извлечение файлов из архива</summary>
        /// <param name="pathToZip"></param>
        public static void ExtractArchive(string pathToZip)
        {
            if (File.Exists(pathToZip))
            {
                using (ZipArchive zipArch = new ZipArchive(File.OpenRead(pathToZip)))
                {
                    foreach (ZipArchiveEntry entry in zipArch.Entries)
                    {
                        if (entry.FullName.Contains("xml"))
                        {
                            entry.ExtractToFile(Path.Combine("C:\\_2", entry.Name), true);
                        }
                    }
                }
            }
        }
    }
}
