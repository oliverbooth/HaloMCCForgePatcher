namespace HaloMCCForgePatcher.Helpers
{
    #region Using Directives

    using System;
    using System.IO;
    using System.Threading.Tasks;

    #endregion

    internal static class FileHelpers
    {
        private static bool cancelBackupCreation = false;

        /// <summary>
        /// Asynchronously creates a backup.
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="percentageChangeCallback"></param>
        /// <returns></returns>
        public static async Task CreateBackupAsync(string sourcePath, Action<double> percentageChangeCallback)
        {
            cancelBackupCreation = false;
            sourcePath           = Path.GetFullPath(sourcePath);

            string destinationPath = Path.GetFullPath($@"{sourcePath}.bak");

            if (!File.Exists(sourcePath))
            {
                return;
            }

            if (File.Exists(destinationPath))
            {
                File.Delete(destinationPath);
            }

            byte[] buffer = new byte[1048576]; // 1 MiB buffer

            await using FileStream source =
                new FileStream(sourcePath, FileMode.Open, FileAccess.Read);
            await using FileStream destination =
                new FileStream(destinationPath, FileMode.CreateNew, FileAccess.Write);

            long length     = source.Length;
            long totalBytes = 0;
            int  currentBlockSize;

            while ((currentBlockSize = source.Read(buffer, 0, buffer.Length)) > 0)
            {
                totalBytes += currentBlockSize;
                double percentage = totalBytes * 100.0 / length;

                destination.Write(buffer, 0, currentBlockSize);
                percentageChangeCallback?.Invoke(percentage);

                if (!cancelBackupCreation)
                {
                    continue;
                }

                // discard backup
                source.Close();
                destination.Close();
                File.Delete(destinationPath);
                cancelBackupCreation = false;
                break;
            }
        }

        public static void CancelBackup()
        {
            cancelBackupCreation = true;
        }
    }
}
