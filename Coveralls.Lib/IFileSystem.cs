using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coveralls
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
