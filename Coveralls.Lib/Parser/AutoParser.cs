using System.Collections.Generic;
using System.Linq;

namespace Coveralls
{
    public class AutoParser : BaseParser
    {
        public override IEnumerable<CoverageFile> Generate()
        {
            if (Report == null || Report.Root == null) return Enumerable.Empty<CoverageFile>();

            var rootElementName = Report.Root.Name.LocalName;

            ICoverageParser parser = null;

            switch (rootElementName)
            {
                case "CoverageSession":
                    parser = new OpenCoverParser();
                    break;
                case "coverage":
                    parser = new CoberturaCoverageParser();
                    break;
            }

            if (parser != null)
            {
                parser.Report = Report;
                return parser.Generate();
            }

            return Enumerable.Empty<CoverageFile>();
        }
    }
}
