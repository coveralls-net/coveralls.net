using System;
using System.Collections.Generic;
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

            var coveralls = new CoverallsBootstrap(Options)
            {
                FileSystem = new LocalFileSystem()
            };

            var coverallsData = new CoverallsData
            {
                ServiceName = coveralls.ServiceName,
                ServiceJobId = coveralls.ServiceJobId,
                RepoToken = coveralls.RepoToken,
                SourceFiles = coveralls.CoverageFiles.ToArray(),
                Git = coveralls.Repository.Data
            };

            // Send to coveralls.io
            HttpResponseMessage response;
            using (var client = new HttpClient())
            {
                using (var formData = new MultipartFormDataContent())
                {
                    var json = JsonConvert.SerializeObject(coverallsData);
                    formData.Add(new StringContent(json), "json_file", "coverage.json");
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
}
