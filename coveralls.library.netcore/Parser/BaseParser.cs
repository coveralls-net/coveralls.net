using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;

namespace Coveralls.Library.Parser
{
    [ExcludeFromCodeCoverage]
    public class BaseParser : ICoverageParser
    {
        public XDocument Report { get; set; }

        public virtual IEnumerable<CoverageFile> Generate()
        {
            return new List<CoverageFile>();
        }
    }
}