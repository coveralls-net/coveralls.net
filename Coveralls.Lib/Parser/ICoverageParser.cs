using System.Collections.Generic;
using System.Xml.Linq;

namespace Coveralls
{
    public interface ICoverageParser
    {
        XDocument Report { get; set; }
        IEnumerable<CoverageFile> Generate();
    }
}