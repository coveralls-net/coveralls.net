using System.IO;
using Coveralls;

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