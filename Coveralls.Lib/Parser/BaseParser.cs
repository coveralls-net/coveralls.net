using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;

namespace Coveralls.Lib.Parser
{
    [ExcludeFromCodeCoverage]
    public class BaseParser : ICoverageParser
    {
        protected IFileSystem FileSystem;
        public BaseParser(IFileSystem fileSystem)
        {
            FileSystem = fileSystem;
        }

        public XDocument Report { get; set; }

        public virtual List<CoverageFile> Generate()
        {
            return new List<CoverageFile>();
        }
    }
}