using System;
using CommandLine;
using Coveralls;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Coveralls.Net
{
    public class CommandLineOptions : ICommandOptions
    {
        private bool _sendFullSources;
        private IFileSystem _fileSystem;
        private List<string> _inputFiles;

        public CommandLineOptions() : this (new LocalFileSystem())
        {}

        public CommandLineOptions(IFileSystem fileSystem)
        {
            if (fileSystem == null) throw new ArgumentNullException("fileSystem");
            _fileSystem = fileSystem;
        }

        [Value(0)]
        public string InputFile { get; set; }

        [Option('f', "pattern", HelpText = "File name search pattern ('*' for wildcards)")]
        public string FileSearchPattern { get; set; }

        public IEnumerable<string> InputFiles
        {
            get
            {
                if (InputFile.IsBlank()) return new List<string>();
                if (_fileSystem.FileExists(InputFile)) return new List<string> { InputFile };

                // backwards compatibility to v1.3.x, cf.: issue #36
                if (InputFile.Contains("*") && FileSearchPattern.IsBlank())
                {
                    FileSearchPattern = InputFile;
                    InputFile = _fileSystem.GetCurrentDirectory();
                }

                if (!_fileSystem.DirectoryExists(InputFile)) return new List<string>(); // cannot find file or directory provided
                if (FileSearchPattern.IsBlank()) return new List<string>(); // pattern must be provided for directories

                return _fileSystem.FileSearch(InputFile, FileSearchPattern, false);
            }
        }

        [Option('p', "parser", HelpText = "Parser to use (Currently supports OpenCover, Cobertura, DotCover and AutoDetect)")]
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

        [Option('s', "full-sources", DefaultValue = false, HelpText="Send full sources instead of the digest" )]
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