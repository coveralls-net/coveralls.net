using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Coveralls.Lib
{
    public class CoverageFile
    {
        private List<string> _sourceLines;
        private Dictionary<int, int> _lineCoverage = new Dictionary<int, int>();

        [JsonProperty("name")]
        public string Path { get; set; }

        [JsonProperty("source")]
        public string Source
        {
            get { return string.Join("\n", _sourceLines); }

            set
            {
                _coverage = null;
                if (string.IsNullOrEmpty(value))
                {
                    _sourceLines = new List<string>();
                    return;
                }

                var lines = new List<string>();
                using (var sr = new StringReader(value))
                {
                    string nextLine;
                    while ((nextLine = sr.ReadLine()) != null)
                    {
                        lines.Add(nextLine);
                    }
                }
                _sourceLines = lines;
            }
        }

        private int?[] _coverage;

        [JsonProperty("coverage")]
        public int?[] Coverage
        {
            get
            {
                if (_coverage == null)
                {
                    var length = 1;
                    if (_sourceLines != null) length = _sourceLines.Count;
                    else if (_lineCoverage.Count > 0) length = _lineCoverage.Max(c => c.Key);

                    _coverage = Enumerable.Range(1, length)
                        .Select(index => _lineCoverage.ContainsKey(index) ? (int?)_lineCoverage[index] : null)
                        .ToArray();
                }
                return _coverage;
            }
        }

        public void Record(int line, int coverage)
        {
            _lineCoverage[line] = coverage;
        }
    }
}