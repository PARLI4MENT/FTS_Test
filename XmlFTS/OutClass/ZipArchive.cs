using System.IO;
using System.IO.Compression;
using XmlFTS.OutClass;

namespace XmlFTS
{
    public static class ArchiveWorker
    {
        /// <summary>Извлекает из ZIP-архива XML-файлы в переименнованном виде</summary>
        /// <param name="pathToZip">Путь к папке с архивами</param>
        public static void ExtractZipArchive(string pathToZip)
        {
            if (File.Exists(pathToZip))
            {
                string code = Path.GetFileName(Path.GetDirectoryName(pathToZip));

                if (!Directory.Exists(Path.Combine(StaticPathConfiguration.PathExtractionFolder)))
                    Directory.CreateDirectory(Path.Combine(StaticPathConfiguration.PathExtractionFolder));

                using (ZipArchive zipArch = new ZipArchive(File.OpenRead(pathToZip)))
                {
                    foreach (ZipArchiveEntry entry in zipArch.Entries)
                    {
                        if (entry.FullName.ToLower().Contains("xml"))
                        {
                            string pathDest = Path.Combine(StaticPathConfiguration.PathExtractionFolder, string.Concat(code, ".", entry.Name));
                            entry.ExtractToFile(pathDest, true);

                            if (Config.EnableBackup)
                                entry.ExtractToFile(Path.Combine(StaticPathConfiguration.PathBackupFolder, string.Concat(code, ".", entry.Name)), true);
                        }
                    }
                }
            }
        }
    }
}
