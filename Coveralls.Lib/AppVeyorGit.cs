using System;
using System.Collections.Generic;

namespace Coveralls
{
    public class AppVeyorGit : GitRepository
    {
        public override IEnumerable<string> Branches { get { return new List<string>() { CurrentBranch }; } }
        public override IEnumerable<CommitData> Commits { get { return new List<CommitData>() { Head }; } }

        private string _branch;
        public override string CurrentBranch 
        { 
            get
            {
                if(string.IsNullOrEmpty(_branch)) _branch = Environment.GetEnvironmentVariable("APPVEYOR_REPO_BRANCH");
                return _branch;
            } 
        }

        private CommitData _head;
        public override CommitData Head
        {
            get
            {
                if (_head == null)
                {
                    _head = new CommitData
                    {
                        Id = Environment.GetEnvironmentVariable("APPVEYOR_REPO_COMMIT"),
                        Message = Environment.GetEnvironmentVariable("APPVEYOR_REPO_COMMIT_MESSAGE"),
                        Author = Environment.GetEnvironmentVariable("APPVEYOR_REPO_COMMIT_AUTHOR"),
                        AuthorEmail = Environment.GetEnvironmentVariable("APPVEYOR_REPO_COMMIT_AUTHOREMAIL"),
                        PullRequestId = Environment.GetEnvironmentVariable("APPVEYOR_PULL_REQUEST_NUMBER")
                    };

                    // AppVeyor only supplies author data
                    _head.Committer = _head.Author;
                    _head.CommitterEmail = _head.AuthorEmail;
                }
                return _head;
            }
        }
    }
}