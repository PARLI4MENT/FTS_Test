using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace XmlFTS.OutClass
{
    internal static class BackupFile
    {
        public static void Backup(string sourcePathFile, [Optional] bool overwrite)
        {
            if (string.IsNullOrEmpty(sourcePathFile))
            {
                ChangeLogger.Log($@"[BACKUP] [INTERNAL] IsNullOrEmpty => {sourcePathFile}", ChangeLogger.LogOperation.ERROR);
                return;
            }
            if (File.Exists(sourcePathFile))
            {
                ChangeLogger.Log($@"[BACKUP] Dont make a backup, source file is not exists => {sourcePathFile}", ChangeLogger.LogOperation.ERROR);
                return;
            }
            File.Copy(sourcePathFile, Path.Combine(StaticPathConfiguration.PathBackupFolder, Path.GetFileName(sourcePathFile)), true);
        }
    }
}
