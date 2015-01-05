using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Coveralls.Lib
{
    public interface ICoverageParser
    {
        List<CoverageFile> Generate();
    }

    public class OpenCoverParser : ICoverageParser
    {
        private IFileSystem _fileSystem;
        public OpenCoverParser(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public XDocument Report { get; set; }

        public List<CoverageFile> Generate()
        {
            return new List<CoverageFile>();
        }
    }
}
