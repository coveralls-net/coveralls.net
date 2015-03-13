using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using CommandLine;
using Coveralls.Net.Properties;
using Newtonsoft.Json;

namespace Coveralls.Net
{
    internal class Program
    {
        internal static CommandLineOptions Options;

        internal static void Main(string[] args)
        {
            Options = Parser.Default.ParseArguments<CommandLineOptions>(args).Value;
            
            if (Options.DebugMode)
            {
                Console.WriteLine(Resources.RepoTokenDebug, Environment.GetEnvironmentVariable("COVERALLS_REPO_TOKEN"));
            }

            try
            {
                var coveralls = new CoverallsBootstrap(Options)
                {
                    FileSystem = new LocalFileSystem()
                };

                // Use specified repo token over Environment variable
                if (Options.CoverallsRepoToken.IsNotBlank())
                    coveralls.RepoToken = Options.CoverallsRepoToken;

                if (!coveralls.CoverageFiles.Any())
                {
                    Console.WriteLine(Resources.NoCoverageFilesErrorMessage);
                    return;
                }

                if (coveralls.RepoToken.IsBlank())
                {
                    Console.WriteLine(Resources.BlankTokenErrorMessage);

                    if (coveralls.ServiceName == "appveyor")
                    {
                        Console.Write(Resources.AppVeyorBlankToken);
                    }
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

                if (Options.DebugMode)
                {
                    Console.Write(Resources.CoverallsDebug, 
                        coverallsData.ServiceName,
                        coverallsData.ServiceJobId,
                        coverallsData.SourceFiles.Count(),
                        coverallsData.Git.Head.Id
                    );
                }

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

        internal static void SendToCoveralls(string json)
        {
            if (Options.DebugMode)
            {
                Console.WriteLine(Resources.CoverallsJsonHeader, JsonPrettyPrint(json));
            }

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
                var msg = string.Format("Error sending to coveralls.io ({0} - {1}).", 
                    response.StatusCode,
                    response.ReasonPhrase);
                msg += "\n - Error code 422 indicate a problem with your token. Try using the --debug commandline option.";

                throw new CoverallsException(msg);
            }
        }

        internal static string JsonPrettyPrint(string json)
        {
            dynamic parsedJson = JsonConvert.DeserializeObject(json);
            return JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
        }
    }
}