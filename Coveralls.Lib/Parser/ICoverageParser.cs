using System.Collections.Generic;
using System.Xml.Linq;

namespace Coveralls.Lib
{
    public interface ICoverageParser
    {
        XDocument Report { get; set; }
        List<CoverageFile> Generate();
    }
}