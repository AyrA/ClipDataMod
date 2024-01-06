using System.Diagnostics.CodeAnalysis;

namespace Plugin
{
    /// <summary>
    /// Provides temporary files with auto cleanup support
    /// </summary>
    /// <remarks>
    /// Plugins must properly dispose of their open file streams for this to work.
    /// Cleanup will be performed on startup and shutdown of the application.
    /// If cleanup fails, it's retried on the next application start
    /// </remarks>
    public class TempFiles : IDisposable
    {
        private static readonly char[] invalids = Path.GetInvalidFileNameChars()
            .Concat(Path.GetInvalidPathChars())
            .Distinct()
            .ToArray();
        private static uint index = 0;
        private readonly string tempDir;

        internal TempFiles()
        {
            tempDir = Path.Combine(Path.GetTempPath(), "ClipDataMod");
            Cleanup();
            try
            {
                Directory.CreateDirectory(tempDir);
            }
            catch (Exception ex)
            {
                throw new Exception($"Unable to create '{tempDir}' as the temporary directory", ex);
            }
        }

        private static uint NextIndex()
        {
            return Interlocked.Increment(ref index);
        }

        private static void CheckName([NotNullWhen(true)] string? name)
        {
            ArgumentException.ThrowIfNullOrEmpty(name);
            if (name.IndexOfAny(invalids) >= 0)
            {
                throw new ArgumentException($"'{name}' contains invalid file name characters");
            }
        }

        /// <summary>
        /// Creates a temporary file with the suggested name
        /// and returns the full path of the created file
        /// </summary>
        /// <param name="fileName">file name. This must not contain path information</param>
        /// <returns>Created file name</returns>
        public string CreateTempFile(string fileName)
        {
            CheckName(fileName);
            fileName = Path.GetFileName(fileName);
            var dirName = Path.Combine(tempDir, NextIndex().ToString());
            var fullPath = Path.Combine(dirName, fileName);
            Directory.CreateDirectory(dirName);
            using var fs = File.Create(fullPath);
            fs.Close();
            return fullPath;
        }

        /// <summary>
        /// Creates a temporary file named <paramref name="fileName"/>,
        /// saves <paramref name="fileData"/> as file content,
        /// then copies the file into the clipboard using "Move" as the preferred action
        /// </summary>
        /// <param name="fileName">File name without path information</param>
        /// <param name="fileData">File data</param>
        /// <returns>Copied full file path</returns>
        public string CreateTempMoveFile(string fileName, byte[] fileData)
        {
            CheckName(fileName);
            ArgumentNullException.ThrowIfNull(fileData);

            using var ms = new MemoryStream(fileData, false);
            return CreateTempMoveFile(fileName, ms);
        }

        /// <summary>
        /// Creates a temporary file named <paramref name="fileName"/>,
        /// copies <paramref name="fileData"/> into the file,
        /// then copies the file into the clipboard using "Move" as the preferred action
        /// </summary>
        /// <param name="fileName">File name without path information</param>
        /// <param name="fileData">File data</param>
        /// <returns>Copied full file path</returns>
        public string CreateTempMoveFile(string fileName, Stream fileData)
        {
            fileName = CreateTempFile(fileName);
            using var fs = File.OpenWrite(fileName);
            fileData.CopyTo(fs);
            PluginHelper.Clipboard.SetFileList([fileName], Clipboard.PreferredFileOperation.Move);
            return fileName;
        }

        /// <summary>
        /// Deletes all temporary files
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Cleanup();
        }

        private void Cleanup()
        {
            try
            {
                //Try the obvious simple way first
                Directory.Delete(tempDir, true);
            }
            catch
            {
                try
                {
                    var files = Directory.GetFiles(tempDir, "*.*", SearchOption.AllDirectories);
                    foreach (var f in files)
                    {
                        try
                        {
                            File.Delete(f);
                        }
                        catch
                        {
                            //NOOP
                        }
                    }
                }
                catch
                {
                    //NOOP
                }
            }
        }
    }
}
