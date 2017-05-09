using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace Coveralls
{
    public class CoberturaCoverageParser : BaseParser
    {
        public override IEnumerable<CoverageFile> Generate()
        {
            var files = new List<CoverageFile>();

            if (Report == null || Report.Root == null) return files;

            var basePathEl = Report.Root.XPathSelectElement("//sources/source");
            var basePath = string.Empty;

            if (basePathEl != null && basePathEl.Value.IsNotBlank())
                basePath = basePathEl.Value;

            foreach (var package in Report.Root.XPathSelectElements("//packages/package"))
            {
                foreach (var cl in package.XPathSelectElements("./classes/class"))
                {
                    var filenameAttrib = cl.Attribute("filename");
                    var fileName = filenameAttrib.Value;
                    var path = Path.Combine(basePath, fileName);

                    var coverageFile = new CoverageFile()
                    {
                        Path = path.ToUnixPath()
                    };

                    foreach (var line in cl.XPathSelectElements("./lines/line"))
                    {
                        var lineNumber = int.Parse(line.Attribute("number").Value);
                        var hits = int.Parse(line.Attribute("hits").Value);

                        coverageFile.Record(lineNumber, hits);
                    }

                    files.Add(coverageFile);
                }
            }

            return files;
        }
    }
}