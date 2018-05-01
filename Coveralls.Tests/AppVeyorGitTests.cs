using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Coveralls;
using FluentAssertions;
using NUnit.Framework;

namespace Coveralls.Tests
{
    [TestFixture]
    public class AppVeyorGitTest
    {
        [SetUp]
        public void Init()
        {
            Environment.SetEnvironmentVariable("APPVEYOR", "true");
            Environment.SetEnvironmentVariable("APPVEYOR_REPO_BRANCH", "");
            Environment.SetEnvironmentVariable("APPVEYOR_REPO_COMMIT", "");
            Environment.SetEnvironmentVariable("APPVEYOR_REPO_COMMIT_MESSAGE", "");
            Environment.SetEnvironmentVariable("APPVEYOR_REPO_COMMIT_AUTHOR", "");
            Environment.SetEnvironmentVariable("APPVEYOR_REPO_COMMIT_AUTHOREMAIL", "");
            Environment.SetEnvironmentVariable("APPVEYOR_PULL_REQUEST_NUMBER", "");
        }

        [Test]
        public void Branch_EnvVariableNotSet_IsNull()
        {
            var git = new AppVeyorGit();
            git.CurrentBranch.Should().Be(null);
        }

        [Test]
        public void Branch_EnvVariableSet_IsCorrectValue()
        {
            Environment.SetEnvironmentVariable("APPVEYOR_REPO_BRANCH", "master");
            var git = new AppVeyorGit();
            git.CurrentBranch.Should().Be("master");
        }

        [Test]
        public void Branches_EnvVariableSet_IsTheOnlyValue()
        {
            Environment.SetEnvironmentVariable("APPVEYOR_REPO_BRANCH", "master");

            var git = new AppVeyorGit();

            git.Branches.Single().Should().Be("master");
        }

        [Test]
        public void Head_EnvVariablesNotSet_HasNullValues()
        {
            var git = new AppVeyorGit();

            git.Head.Id.Should().Be(null);
            git.Head.Message.Should().Be(null);
            git.Head.Author.Should().Be(null);
            git.Head.AuthorEmail.Should().Be(null);
        }

        [Test]
        public void Head_EnvVariablesSet_HasCorrectValues()
        {
            Environment.SetEnvironmentVariable("APPVEYOR_REPO_COMMIT", "1234abcd");
            Environment.SetEnvironmentVariable("APPVEYOR_REPO_COMMIT_MESSAGE", "Initial commit");
            Environment.SetEnvironmentVariable("APPVEYOR_REPO_COMMIT_AUTHOR", "jdeering");
            Environment.SetEnvironmentVariable("APPVEYOR_REPO_COMMIT_AUTHOREMAIL", "jason@deering.me");

            var git = new AppVeyorGit();

            git.Head.Id.Should().Be("1234abcd");
            git.Head.Message.Should().Be("Initial commit");
            git.Head.Author.Should().Be("jdeering");
            git.Head.AuthorEmail.Should().Be("jason@deering.me");
            git.Head.Committer.Should().Be("jdeering");
            git.Head.CommitterEmail.Should().Be("jason@deering.me");
        }

        [Test]
        public void Commits_EnvVariablesSet_OnlyContainsHead()
        {
            Environment.SetEnvironmentVariable("APPVEYOR_REPO_COMMIT", "1234abcd");
            Environment.SetEnvironmentVariable("APPVEYOR_REPO_COMMIT_MESSAGE", "Initial commit");
            Environment.SetEnvironmentVariable("APPVEYOR_REPO_COMMIT_AUTHOR", "jdeering");
            Environment.SetEnvironmentVariable("APPVEYOR_REPO_COMMIT_AUTHOREMAIL", "jason@deering.me");

            var git = new AppVeyorGit();

            git.Commits.Single().Should().Be(git.Head);
        }

        [Test]
        public void Head_PullRequestNumberSet_HasCorrectValue()
        {
            Environment.SetEnvironmentVariable("APPVEYOR_PULL_REQUEST_NUMBER", "1234abcd");

            var git = new AppVeyorGit();

            git.Head.PullRequestId.Should().Be("1234abcd");
        }
    }
}