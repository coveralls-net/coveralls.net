using System.Collections.Generic;

namespace Coveralls.Library
{
    public interface IFileSystem
    {
        string ReadFileText(string path);
        byte[] ComputeHash(string path);
        string GetCurrentDirectory();
        bool FileExists(string path);
        bool DirectoryExists(string path);
        IEnumerable<string> FileSearch(string directory, string fileSearchPattern);
        IEnumerable<string> FileSearch(string directory, string fileSearchPattern, bool recursive);
    }
}
