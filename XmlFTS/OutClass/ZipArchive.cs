using System;
using System.IO;
using System.IO.Compression;

namespace XmlFTS.OutClass
{
    public static class ArchiveWorker
    {
        /// <summary> Извлечение файлов из архива</summary>
        /// <param name="pathToZip"></param>
        /// <param name="destinationDirectory">Папка назначения</param>
        public static void ExtractZipArchive(string pathToZip, string destinationDirectory)
        {
            if (!Directory.Exists(destinationDirectory))
                Directory.CreateDirectory(destinationDirectory);

            if (File.Exists(pathToZip))
                using (ZipArchive zipArch = new ZipArchive(File.OpenRead(pathToZip)))
                {
                    foreach (ZipArchiveEntry entry in zipArch.Entries)
                    {
                        if (entry.FullName.Contains("xml"))
                            entry.ExtractToFile(Path.Combine(destinationDirectory, entry.Name), true);
                    }
                }
        }
    }
}
