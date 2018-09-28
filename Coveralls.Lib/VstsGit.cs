using System;
using System.Collections.Generic;

namespace Coveralls
{
    public class VstsGit : GitRepository
    {
        public override IEnumerable<string> Branches { get { return new List<string>() { CurrentBranch }; } }
        public override IEnumerable<CommitData> Commits { get { return new List<CommitData>() { Head }; } }

        private string _branch;
        public override string CurrentBranch 
        { 
            get
            {
                if(string.IsNullOrEmpty(_branch)) _branch = Environment.GetEnvironmentVariable("BUILD_SOURCEBRANCHNAME");
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
                        Id = Environment.GetEnvironmentVariable("BUILD_SOURCEVERSION"),
                        Author = Environment.GetEnvironmentVariable("BUILD_REQUESTEDFOR"),
                        AuthorEmail = Environment.GetEnvironmentVariable("BUILD_REQUESTEDFOREMAIL"),
                        Message = Environment.GetEnvironmentVariable("BUILD_SOURCEVERSIONMESSAGE"),
                        PullRequestId = Environment.GetEnvironmentVariable("SYSTEM_PULLREQUEST_PULLREQUESTID")
                    };

                    if (string.IsNullOrWhiteSpace(_head.PullRequestId))
                    {
                        _head.PullRequestId = Environment.GetEnvironmentVariable("SYSTEM_PULLREQUEST_PULLREQUESTNUMBER");
                    }
                }
                return _head;
            }
        }
    }
}