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
        internal static void Main(string[] args)
        {
            try
            {
                Parser.Default.ParseArguments<CommandLineOptions>(args).WithParsed(Run);
            }
            catch (Exception e)
            {
                Console.WriteLine(Resources.GenericError);
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                Environment.Exit(1);
            }
        }

        internal static void Run(CommandLineOptions options)
        {
            if (options.DebugMode)
            {
                Console.WriteLine(Resources.RepoTokenDebug, Environment.GetEnvironmentVariable("COVERALLS_REPO_TOKEN"));
            }

            using (var coveralls = new CoverallsBootstrap(options))
            {
                coveralls.FileSystem = new LocalFileSystem();

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

                foreach (var file in coveralls.CoverageFiles)
                {
                    file.Path = file.Path.ToRelativePath(Directory.GetCurrentDirectory());
                }

                var coverallsData = coveralls.GetData();
                var json = JsonConvert.SerializeObject(coverallsData);
                SendToCoveralls(json);

                if (options.DebugMode)
                {
                    Console.WriteLine(Resources.CoverallsJsonHeader, JsonPrettyPrint(json));
                }
            }
        }

        internal static bool SendToCoveralls(string json)
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
                var msg = string.Format("Error {0} sending to coveralls.io: {1}", 
                    response.StatusCode,
                    response.ReasonPhrase);
                msg += "\n - Error code 422 indicate a problem with your token. Use the --debug option for more details.";
                Console.WriteLine(msg);
            }

            return response.IsSuccessStatusCode;
        }

        internal static string JsonPrettyPrint(string json)
        {
            dynamic parsedJson = JsonConvert.DeserializeObject(json);
            return JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
        }
    }
}