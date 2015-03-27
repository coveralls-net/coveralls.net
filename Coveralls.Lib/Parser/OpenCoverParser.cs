using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Coveralls
{
    public class OpenCoverParser : BaseParser
    {
        public OpenCoverParser()
        {
        }

        public override IEnumerable<CoverageFile> Generate()
        {
            var files = new List<CoverageFile>();

            if (Report == null || Report.Root == null) return files;

            foreach (var module in Report.Root.XPathSelectElements("//Modules/Module"))
            {
                var skippedAttr = module.Attribute("skippedDueTo");
                if (skippedAttr != null && skippedAttr.Value.IsNotBlank()) continue;

                foreach (var file in module.XPathSelectElements("./Files/File"))
                {
                    var fileid = file.Attribute("uid").Value;
                    var fullPath = file.Attribute("fullPath").Value;

                    var coverageFile = new CoverageFile
                    {
                        Path = fullPath.ToUnixPath()
                    };

                    foreach (var @class in module.XPathSelectElements("./Classes/Class"))
                    {
                        foreach (var method in @class.XPathSelectElements("./Methods/Method"))
                        {
                            foreach(var sequencePoint in method.XPathSelectElements("./SequencePoints/SequencePoint"))
                            {
                                var sequenceFileId = sequencePoint.Attribute("fileid").Value;
                                if (fileid == sequenceFileId)
                                {
                                    var line = int.Parse(sequencePoint.Attribute("sl").Value, CultureInfo.CurrentCulture);
                                    var visits = int.Parse(sequencePoint.Attribute("vc").Value, CultureInfo.CurrentCulture);

                                    coverageFile.Record(line, visits);
                                }
                            }
                        }
                    }

                    files.Add(coverageFile);
                }
            }

            return files;
        }
    }
}
