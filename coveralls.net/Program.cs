using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using CommandLine;
using Coveralls.Lib;
using Newtonsoft.Json;

namespace coveralls.net
{
    public enum ParserTypes
    {
        OpenCover,
    }

    public class LocalFileSystem : IFileSystem
    {
        public string ReadFileText(string path)
        {
            if (!Path.IsPathRooted(path))
                path = Directory.GetCurrentDirectory() + "\\" + path;

            if (File.Exists(path))
            {
                var content = File.ReadAllText(path);
                return content;
            }
            return null;
        }
    }

    public class Program
    {
        static void Main(string[] args)
        {
            ICoverageParser parser;
            var fileSystem = new LocalFileSystem();
            var opts = Parser.Default.ParseArguments<CommandLineOptions>(args).Value;

            switch (opts.Parser)
            {
                case ParserTypes.OpenCover:
                    parser = new OpenCoverParser(fileSystem);
                    break;
                default:
                    parser = new OpenCoverParser(fileSystem);
                    break;
            }
            var reportXml = fileSystem.ReadFileText(opts.InputFile);
            if (reportXml.IsBlank())
            {
                Console.Error.WriteLine("Coveralls - Invalid Report File");
                Environment.Exit(1);
            }

            parser.Report = XDocument.Parse(reportXml);
            var coverageFiles = parser.Generate();

            // Job and Commit Data
            var gitData = new GitData
            {
                Branch = Environment.GetEnvironmentVariable(""),
                Head = new CommitData
                {
                    Id = Environment.GetEnvironmentVariable("APPVEYOR_REPO_COMMIT"),
                    Message = Environment.GetEnvironmentVariable("APPVEYOR_REPO_COMMIT_MESSAGE"),
                    Author = Environment.GetEnvironmentVariable("APPVEYOR_REPO_COMMIT_AUTHOR"),
                    AuthorEmail = Environment.GetEnvironmentVariable("APPVEYOR_REPO_COMMIT_AUTHOREMAIL"),
                }
            };

            var coverallsData = new CoverallsData
            {
                ServiceName = "appveyor",
                ServiceJobId = Environment.GetEnvironmentVariable("APPVEYOR_JOB_ID") ?? "0",
                RepoToken = Environment.GetEnvironmentVariable("COVERALLS_REPO_TOKEN"),
                SourceFiles = coverageFiles.ToArray(),
                Git = gitData
            };

            // Send to coveralls.io
            HttpResponseMessage response;
            using (var client = new HttpClient())
            {
                using (var formData = new MultipartFormDataContent())
                {
                    var json = JsonConvert.SerializeObject(coverallsData);
                    Console.Error.WriteLine(json);
                    var content = new StringContent(json);
                    formData.Add(content, "json_file", "coverage.json");
                    response = client.PostAsync(@"https://coveralls.io/api/v1/jobs", formData).Result;
                }
            }

            if (!response.IsSuccessStatusCode)
            {
                Console.Error.WriteLine("Coveralls - Send Error");
                Console.Error.WriteLine(response.ReasonPhrase);
                Environment.Exit(2);
            }

            Environment.Exit(0);
        }
    }

    internal class CommandLineOptions
    {
        [Value(0)]
        public string InputFile { get; set; }

        [Option('p', "parser", HelpText = "Parser to use (Currently only supports OpenCover)")]
        public ParserTypes Parser { get; set; }

        private bool _useOpenCover;
        [Option("opencover")]
        public bool UseOpenCover
        {
            get { return _useOpenCover; }
            set
            {
                _useOpenCover = value;
                if (_useOpenCover) Parser = ParserTypes.OpenCover;
            }
        }
    }
}
