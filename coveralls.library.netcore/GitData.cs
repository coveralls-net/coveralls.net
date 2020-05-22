using System;
using Newtonsoft.Json;

namespace Coveralls
{
    public sealed class GitData
    {
        [JsonProperty("head", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public CommitData Head { get; set; }

        [JsonProperty("branch", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Branch { get; set; }

        [JsonProperty("remotes", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public GitRemotes Remotes { get; set; }
    }

    public sealed class CommitData
    {
        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Id { get; set; }

        [JsonProperty("author_name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Author { get; set; }

        [JsonProperty("author_email", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string AuthorEmail { get; set; }

        [JsonProperty("committer_name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Committer { get; set; }

        [JsonProperty("committer_email", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string CommitterEmail { get; set; }

        [JsonProperty("message", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Message { get; set; }

        public string PullRequestId { get; set; }
    }

    public sealed class GitRemotes
    {
        [JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("url", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Uri Uri { get; set; }
    }
}