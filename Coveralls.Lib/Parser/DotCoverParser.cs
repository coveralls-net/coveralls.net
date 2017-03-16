using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Coveralls
{
    public class DotCoverParser : BaseParser
    {
        public override IEnumerable<CoverageFile> Generate()
        {
            var files = new List<CoverageFile>();

            if (Report == null || Report.Root == null) return files;

            if (Report.Root.Attribute("ReportType")?.Value != "DetailedXml")
                throw new InvalidDataException("dotCover reports must be in 'DetailedXml' format");

            var paths = new List<string>();
            foreach (var file in Report.Root.XPathSelectElements("//Root/FileIndices/File"))
            {
                var name = file.Attribute("Name");
                if (name != null && name.Value.IsNotBlank())
                    paths.Add(name.Value);
            }

            // Woohoo!
            var lineVisitsPerFile = new Dictionary<int, Dictionary<int, int>>();

            foreach (var statement in Report.Root.XPathSelectElements("//Root/Assembly/Namespace/Type/Method/Statement"))
            {
                var covered = BoolAttributeValue(statement, "Covered");
                var fileIndex = IntAttributeValue(statement, "FileIndex");
                var startLine = IntAttributeValue(statement, "Line");
                var endLine = IntAttributeValue(statement, "EndLine");

                if (!lineVisitsPerFile.ContainsKey(fileIndex))
                    lineVisitsPerFile[fileIndex] = new Dictionary<int, int>();

                var lineVisitsForThisFile = lineVisitsPerFile[fileIndex];

                for (var line = startLine; line <= endLine; line++)
                {
                    if (!lineVisitsForThisFile.ContainsKey(line))
                        lineVisitsForThisFile[line] = 0;
                    if(covered)
                        lineVisitsForThisFile[line]++;
                }
            }


            foreach (var file in lineVisitsPerFile)
            {
                var path = paths[file.Key - 1];

                var coverageFile = new CoverageFile
                {
                    Path = path.ToUnixPath()
                };
                foreach (var visit in file.Value)
                {
                    coverageFile.Record(visit.Key, visit.Value);
                }
                files.Add(coverageFile);
            }

            return files;
        }

        private int IntAttributeValue(XElement element, string attributeName)
        {
            var attribute = element.Attribute(attributeName);
            if (attribute == null)
                throw new InvalidDataException($"Empty {attributeName} attribute for {element.Name}");

            return int.Parse(attribute.Value);
        }

        private bool BoolAttributeValue(XElement element, string attributeName)
        {
            var attribute = element.Attribute(attributeName);
            if (attribute == null)
                throw new InvalidDataException($"Empty {attributeName} attribute for {element.Name}");

            return bool.Parse(attribute.Value);
        }
    }
}