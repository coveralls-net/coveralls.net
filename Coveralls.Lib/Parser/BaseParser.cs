using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;

namespace Coveralls
{
    [ExcludeFromCodeCoverage]
    public class BaseParser : ICoverageParser
    {
        private IFileSystem _fileSystem;
        protected IFileSystem FileSystem
        {
            get { return _fileSystem; }
        }
        public BaseParser(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public XDocument Report { get; set; }

        public virtual IEnumerable<CoverageFile> Generate()
        {
            return new List<CoverageFile>();
        }
    }
}