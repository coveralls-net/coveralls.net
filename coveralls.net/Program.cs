using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using CommandLine;
using Coveralls.Lib;
using Newtonsoft.Json;

namespace coveralls.net
{
    public class Program
    {
        public static CommandLineOptions Options;

        static void Main(string[] args)
        {
            Options = Parser.Default.ParseArguments<CommandLineOptions>(args).Value;

            try
            {
                var coveralls = new CoverallsBootstrap(Options)
                {
                    FileSystem = new LocalFileSystem()
                };

                if (!coveralls.CoverageFiles.Any())
                {
                    Console.WriteLine("No coverage statistics files.");
                    return;
                }

                if (Options.DebugMode)
                {
                    Console.WriteLine("[debug] repotoken: '" + coveralls.RepoToken + "'");
                }

                if (coveralls.RepoToken.IsBlank())
                {
                    Console.WriteLine("Blank or invalid Coveralls Repo Token. "
                        + "Did you prefix your token with 'secure:' without encrypting it?");
                    return;
                }

                var sourceFiles = coveralls.CoverageFiles;
                foreach (var file in sourceFiles)
                    file.Path = file.Path.ToRelativePath(Directory.GetCurrentDirectory());

                var coverallsData = new CoverallsData
                {
                    ServiceName = coveralls.ServiceName,
                    ServiceJobId = coveralls.ServiceJobId,
                    RepoToken = coveralls.RepoToken,
                    SourceFiles = coveralls.CoverageFiles.ToArray(),
                    Git = coveralls.Repository.Data
                };

                Console.WriteLine("     Service: {0}", coverallsData.ServiceName);
                Console.WriteLine("      Job ID: {0}", coverallsData.ServiceJobId);
                Console.WriteLine("       Files: {0}", coverallsData.SourceFiles.Length);
                Console.WriteLine("      Commit: {0}", coverallsData.Git.Head.Id);
                Console.WriteLine("Pull Request: {0}", coverallsData.Git.Head.Id);

                var json = JsonConvert.SerializeObject(coverallsData);
                SendToCoveralls(json);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                if (e.InnerException != null)
                    Console.Error.WriteLine(e.InnerException.Message);

                Environment.Exit(1);
            }
        }

        private static void SendToCoveralls(string json)
        {
            // Send to coveralls.io
            HttpResponseMessage response;
            using (var client = new HttpClient())
            {
                using (var formData = new MultipartFormDataContent())
                {
                    formData.Add(new StringContent(json), "json_file", "coverage.json");
                    response = client.PostAsync(@"https://coveralls.io/api/v1/jobs", formData).Result;
                }
            }

            if (!response.IsSuccessStatusCode)
            {
                var msg = "Error sending to Coveralls.io ({0} - {1})."
                             + "\nError code 422 indicate a problem with your token. Try using the --debug commandline option.";
                throw new Exception(string.Format(msg, response.StatusCode, response.ReasonPhrase));
            }
        }
    }
}
