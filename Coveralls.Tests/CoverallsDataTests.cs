using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using Coveralls;
using FluentAssertions;
using NUnit.Framework;
using Newtonsoft.Json;

namespace Coveralls.Tests
{
    [TestFixture]
    public class CoverallsDataTests
    {
        [SetUp]
        public void Init()
        {
            Environment.SetEnvironmentVariable("APPVEYOR_PULL_REQUEST_NUMBER", null);
        }

        [Test]
        public void CoverallsData_PullRequestId_PullsFromGitData()
        {
            Environment.SetEnvironmentVariable("APPVEYOR_PULL_REQUEST_NUMBER", "b31f08d07ae564b08237e5a336e478b24ccc4a65");
            var git = new AppVeyorGit();

            var coverallsData = new CoverallsData
            {
                ServiceName = "ServiceName",
                ServiceJobId = "JobId",
                RepoToken = "b31f08d07ae564b08237e5a336e478b24ccc4a65",
                SourceFiles = (new List<CoverageFile>()).ToArray(),
                Git = git.Data
            };

            coverallsData.ServicePullRequest.Should().Be("b31f08d07ae564b08237e5a336e478b24ccc4a65");
        }

        [Test]
        public void CoverallsData_NullPullRequestId_IsNotInJSON()
        {
            var git = new AppVeyorGit();

            var coverallsData = new CoverallsData
            {
                ServiceName = "ServiceName",
                ServiceJobId = "JobId",
                RepoToken = "b31f08d07ae564b08237e5a336e478b24ccc4a65",
                SourceFiles = (new List<CoverageFile>()).ToArray(),
                Git = git.Data
            };

            var json = JsonConvert.SerializeObject(coverallsData);

            json.Should().NotContain("service_pull_request");
        }

        [Test]
        public void CoverallsData_BlankPullRequestId_IsNotInJSON()
        {
            Environment.SetEnvironmentVariable("APPVEYOR_PULL_REQUEST_NUMBER", "");
            var git = new AppVeyorGit();

            var coverallsData = new CoverallsData
            {
                ServiceName = "ServiceName",
                ServiceJobId = "JobId",
                RepoToken = "b31f08d07ae564b08237e5a336e478b24ccc4a65",
                SourceFiles = (new List<CoverageFile>()).ToArray(),
                Git = git.Data
            };

            var json = JsonConvert.SerializeObject(coverallsData);

            json.Should().NotContain("service_pull_request");
        }

        [Test]
        public void CoverallsData_FilledPullRequestId_IsInJSON()
        {
            Environment.SetEnvironmentVariable("APPVEYOR_PULL_REQUEST_NUMBER", "b31f08d07ae564b08237e5a336e478b24ccc4a65");
            var git = new AppVeyorGit();

            var coverallsData = new CoverallsData
            {
                ServiceName = "ServiceName",
                ServiceJobId = "JobId",
                RepoToken = "b31f08d07ae564b08237e5a336e478b24ccc4a65",
                SourceFiles = (new List<CoverageFile>()).ToArray(),
                Git = git.Data
            };

            var json = JsonConvert.SerializeObject(coverallsData);

            json.Should().Contain("service_pull_request");
        }
    }
}