using System.IO;
using Coveralls.Lib;

namespace coveralls.net
{
    public class LocalFileSystem : IFileSystem
    {
        public string ReadFileText(string path)
        {
            if (!Path.IsPathRooted(path))
                path = Directory.GetCurrentDirectory() + "\\" + path;

            if (File.Exists(path))
            {
                var content = File.ReadAllText(path);
                return content;
            }
            return null;
        }
    }
}