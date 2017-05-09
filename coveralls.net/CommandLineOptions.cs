using CommandLine;
using Coveralls;
using System.Collections.Generic;
using System.Linq;

namespace Coveralls.Net
{
    public class CommandLineOptions : ICommandOptions
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

        [Option('p', "parser", HelpText = "Parser to use (Currently supports OpenCover, Cobertura and AutoDetect)")]
        public ParserType Parser { get; set; }

        [Option('d', "debug", HelpText = "Debug mode. WILL PRINT SENSITIVE DATA")]
        public bool DebugMode { get; set; }

        private bool _useOpenCover;
        [Option("opencover", HelpText = "Use the OpenCover Parser")]
        public bool UseOpenCover
        {
            get { return _useOpenCover; }
            set
            {
                _useOpenCover = value;
                if (_useOpenCover) Parser = ParserType.OpenCover;
            }
        }

        private bool _useCobertura;
        [Option("cobertura", HelpText = "Use the Cobertura Parser")]
        public bool UseCobertura
        {
            get { return _useCobertura; }
            set
            {
                _useCobertura = value;
                if (_useCobertura) Parser = ParserType.Cobertura;
            }
        }

        private bool _useAutoDetect;
        [Option("autodetect", HelpText = "Use the AutoDetect Parser (chooses parser based on input file)")]
        public bool UseAutoDetect
        {
            get { return _useAutoDetect; }
            set
            {
                _useAutoDetect = value;
                if (_useAutoDetect) Parser = ParserType.AutoDetect;
            }
        }

        private bool _sendFullSources;
        [Option('f', "full-sources", DefaultValue = false, HelpText="Send full sources instead of the digest" )]
        public bool SendFullSources
        {
            get { return _sendFullSources; }
            set
            {
                _sendFullSources = value;
            }
        }

        [Option('r', "repo-token")]
        public string CoverallsRepoToken { get; set; }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof (CommandLineOptions)) return false;

            var other = (CommandLineOptions)obj;

            if (this.InputFiles.Count() != other.InputFiles.Count())
            {
                return false;
            }

            for (int i = 0; i < InputFiles.Count(); i++)
            {
                if (this.InputFiles.ElementAt(i) != other.InputFiles.ElementAt(i))
                {
                    return false;
                }
            }

            return
                this.Parser == other.Parser &&
                this.DebugMode == other.DebugMode &&
                this.UseOpenCover == other.UseOpenCover &&
                this.UseCobertura == other.UseCobertura &&
                this.CoverallsRepoToken == other.CoverallsRepoToken;
        }

        public override int GetHashCode()
        {
            var hash = 11;
            foreach (string inputFile in InputFiles)
            {
                hash = (hash * 7) + inputFile.GetHashCode();
            }
            
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