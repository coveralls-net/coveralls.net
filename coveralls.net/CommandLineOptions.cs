using CommandLine;
using System.Collections.Generic;
using Coveralls.Lib;
using System.Linq;

namespace coveralls.net
{
    internal class CommandLineOptions : ICommandOptions
    {
        private List<string> _inputFiles;
        [Value(0)]
        public IEnumerable<string> InputFiles
        {
            get { return _inputFiles; }
            set
            {
                // Alter the input list to expand wildcards

                if (value != null && value.Any())
                {
                    _inputFiles = new List<string>();

                    foreach (string input in value)
                    {
                        string fileName = System.IO.Path.GetFileName(input);
                        string path = System.IO.Path.GetDirectoryName(input);

                        if (string.IsNullOrEmpty(path))
                        {
                            path = System.Environment.CurrentDirectory;
                        }

                        _inputFiles.AddRange(System.IO.Directory.GetFiles(path, fileName));
                    }
                }
            }
        }

        [Option('p', "parser", HelpText = "Parser to use (Currently only supports OpenCover)")]
        public ParserType Parser { get; set; }

        [Option('d', "debug", HelpText = "Debug mode. WILL PRINT SENSITIVE DATA")]
        public bool DebugMode { get; set; }

        private bool _useOpenCover;
        [Option("opencover")]
        public bool UseOpenCover
        {
            get { return _useOpenCover; }
            set
            {
                _useOpenCover = value;
                if (_useOpenCover) Parser = ParserType.OpenCover;
            }
        }

        [Option("repo-token")]
        public string CoverallsRepoToken { get; set; }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof (CommandLineOptions)) return false;

            var other = (CommandLineOptions)obj;

            return this.InputFile == other.InputFile &&
                this.Parser == other.Parser &&
                this.DebugMode == other.DebugMode &&
                this.UseOpenCover == other.UseOpenCover &&
                this.CoverallsRepoToken == other.CoverallsRepoToken;
        }

        public override int GetHashCode()
        {
            var hash = 11;
            hash = (hash * 7) + InputFile.GetHashCode();
            hash = (hash * 7) + Parser.GetHashCode();
            hash = (hash * 7) + DebugMode.GetHashCode();
            hash = (hash * 7) + CoverallsRepoToken.GetHashCode();
            return hash;
        }

        public static bool operator ==(CommandLineOptions a, CommandLineOptions b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(CommandLineOptions a, CommandLineOptions b)
        {
            return !a.Equals(b);
        }
    }
}