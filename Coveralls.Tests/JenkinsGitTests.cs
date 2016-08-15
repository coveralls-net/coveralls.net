using System;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using Coveralls;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace Coveralls.Tests
{
    [TestFixture]
    public class JenkinsGitTests
    {
        IFileSystem fileSystem = Stub.LocalFileSystem();

        [SetUp]
        public void Init()
        {
            Environment.SetEnvironmentVariable("GIT_BRANCH", "");
        }

        [Test]
        public void Branch_EnvVariableSet_PullsValue()
        {
            Environment.SetEnvironmentVariable("GIT_BRANCH", "master");

            var git = new JenkinsGit(fileSystem);

            git.CurrentBranch.Should().Be("master");
        }

        [Test]
        public void Branch_EnvVariableSet_ParsesBranchFromRemote()
        {
            Environment.SetEnvironmentVariable("GIT_BRANCH", "origin/master");

            var git = new JenkinsGit(fileSystem);

            git.CurrentBranch.Should().Be("master");
        }

        [Test]
        public void Branch_EnvVariableSet_AssumesMasterOnPullRequest()
        {
            Environment.SetEnvironmentVariable("GIT_BRANCH", "origin/pr/1234/merge");

            var git = new JenkinsGit(fileSystem);

            git.CurrentBranch.Should().Be("master");
        }

        [Test]
        public void Head_PullRequestNumberSet_HasCorrectValue()
        {
            Environment.SetEnvironmentVariable("GIT_BRANCH", "origin/pr/1234");

            var git = new JenkinsGit(fileSystem);

            git.Head.PullRequestId.Should().Be("1234");
        }
    }
}