using System.Collections.Generic;
using Newtonsoft.Json;

namespace Coveralls
{
    public sealed class CoverallsData
    {
        [JsonProperty("service_name")]
        public string ServiceName { get; set; }

        [JsonProperty("service_job_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string ServiceJobId { get; set; }

        [JsonProperty("repo_token")]
        public string RepoToken { get; set; }

        [JsonProperty("source_files")]
        public IEnumerable<CoverageFile> SourceFiles { get; set; }

        [JsonProperty("git", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public GitData Git { get; set; }

        [JsonProperty("service_pull_request", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string ServicePullRequest
        {
            get
            {
                return Git != null ? Git.Head.PullRequestId : null;
            }
        }
    }
}