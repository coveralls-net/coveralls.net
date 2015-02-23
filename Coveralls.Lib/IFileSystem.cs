using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coveralls
{
    public interface IFileSystem
    {
        string ReadFileText(string path);
    }
}
