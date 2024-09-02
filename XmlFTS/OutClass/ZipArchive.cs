using System;
using System.IO;
using System.IO.Compression;

namespace XmlFTS.OutClass
{
    public static class ArchiveWorker
    {
        /// <summary> Извлечение файлов из архива</summary>
        /// <param name="pathToZip"></param>
        /// <param name="dirDestination">Папка назначения</param>
        /// <remarks> Папка назначения содержит папку "code" </remarks>
        public static void ExtractZipArchive(string pathToZip, string dirDestination = "C:\\_2\\ExtractionFiles")
        {
            if (File.Exists(pathToZip))
            {
                string code = Path.GetDirectoryName(pathToZip);

                if (!Directory.Exists(Path.Combine(dirDestination, code)))
                    Directory.CreateDirectory(Path.Combine(dirDestination, code));

                using (ZipArchive zipArch = new ZipArchive(File.OpenRead(pathToZip)))
                {
                    foreach (ZipArchiveEntry entry in zipArch.Entries)
                    {
                        if (entry.FullName.Contains("xml"))
                            entry.ExtractToFile(Path.Combine(dirDestination, code, string.Concat(code, ".", entry.Name)), true);
                    }
                }
            }
        }
    }
}
