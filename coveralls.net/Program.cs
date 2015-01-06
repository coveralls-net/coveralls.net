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

            try
            {
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

                var json = JsonConvert.SerializeObject(coverallsData);
                SendToCoveralls(json);

                Environment.Exit(0);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
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
                throw new Exception(string.Format("Error sending to Coveralls.io ({0} - {1}", response.StatusCode, response.ReasonPhrase));
            }
        }
    }
}
