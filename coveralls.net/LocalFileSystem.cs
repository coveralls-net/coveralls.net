using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using Coveralls;

namespace Coveralls.Net
{
    internal class LocalFileSystem : IFileSystem
    {
        public string ReadFileText(string path)
        {
            var rootedPath = RootedPath(path);

            if (File.Exists(rootedPath))
            {
                var content = File.ReadAllText(rootedPath);
                return content;
            }
            return null;
        }

        public byte[] ComputeHash(string path)
        {
            var rootedPath = RootedPath(path);

            using (var md5 = MD5.Create())
            {
                using (var stream = new BufferedStream(File.OpenRead(rootedPath), 1200000))
                {
                    return md5.ComputeHash(stream);
                }
            }
        }

        public string GetCurrentDirectory()
        {
            return Directory.GetCurrentDirectory();
        }

        private string RootedPath(string path)
        {
            return Path.IsPathRooted(path) ? path : string.Format(@"{0}\{1}", GetCurrentDirectory(), path);
        }

        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public IEnumerable<string> FileSearch(string directory, string fileSearchPattern)
        {
            return FileSearch(directory, fileSearchPattern, false);
        }
        
        public IEnumerable<string> FileSearch(string directory, string fileSearchPattern, bool recursive)
        {
            var searchOption = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            return Directory.GetFiles(directory, fileSearchPattern, searchOption);
        }
    }
}