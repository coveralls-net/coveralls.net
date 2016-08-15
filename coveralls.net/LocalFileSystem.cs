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
    }
}