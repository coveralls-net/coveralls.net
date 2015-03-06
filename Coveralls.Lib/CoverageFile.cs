using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace Coveralls
{
    public class CoverageFile
    {
        private string _source;
        private int _sourceLineCount;
        private Dictionary<int, int> _lineCoverage = new Dictionary<int, int>();

        [JsonProperty("name")]
        public string Path { get; set; }

        [JsonProperty("source")]
        public string Source
        {
            get { return _source; }
            set
            {
                _coverage = null;
                _sourceLineCount = 0;
                if (string.IsNullOrEmpty(value)) {
                    _source = null;
                    return;
                }

                var lines = new List<string>();
                using (var sr = new StringReader(value)) {
                    string nextLine;
                    while ((nextLine = sr.ReadLine()) != null) {
                        lines.Add(nextLine);
                    }
                }
                _sourceLineCount = lines.Count;
                _source = string.Join("\n", lines);
            }
        }

        [JsonProperty("source_digest")]
        public string SourceDigest
        {
            get
            {
                MD5 hash = MD5.Create();
                byte[] md5Digest = hash.ComputeHash(Encoding.UTF8.GetBytes(_source));

                // Convert to string
                StringBuilder builder = new StringBuilder();

                foreach (byte b in md5Digest) {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        // Special functions to determine whether the Source or SourceDigest properties should
        // get serialized

        public bool ShouldSerializeSourceDigest()
        {
            return _digest;
        }

        public bool ShouldSerializeSource()
        {
            return !_digest;
        }

        private bool _digest = true;

        [JsonIgnore]
        public bool Digest
        { 
            get 
            { 
                return _digest; 
            }
            set 
            { 
                _digest = value; 
            }
        }

        private IEnumerable<int?> _coverage;

        [JsonIgnore]
        public int SourceLineCount { get { return _sourceLineCount; } }

        [JsonProperty("coverage")]
        public IEnumerable<int?> Coverage
        {
            get
            {
                if (_coverage == null)
                {
                    var length = 1;
                    if (SourceLineCount > 0) length = SourceLineCount;
                    else if (_lineCoverage.Count > 0) length = _lineCoverage.Max(c => c.Key);

                    _coverage = Enumerable.Range(1, length)
                        .Select(index => _lineCoverage.ContainsKey(index) ? (int?)_lineCoverage[index] : null);
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