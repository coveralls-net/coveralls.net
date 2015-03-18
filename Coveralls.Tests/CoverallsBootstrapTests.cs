using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace Coveralls.Tests
{
    [TestFixture]
    public class CoverallsBootstrapTests
    {
        [SetUp]
        public void Init()
        {
            Environment.SetEnvironmentVariable("APPVEYOR", "");
            Environment.SetEnvironmentVariable("APPVEYOR_JOB_ID", "");
            Environment.SetEnvironmentVariable("JENKINS_HOME", "");
            Environment.SetEnvironmentVariable("BUILD_NUMBER", "");
            Environment.SetEnvironmentVariable("COVERALLS_REPO_TOKEN", "");
        }

        [Test]
        public void Parser_NoOptionsSpecified_UsesOpenCoverParser()
        {
            var opts = Substitute.For<ICommandOptions>();
            var coveralls = new CoverallsBootstrap(opts);

            coveralls.CreateParser().Should().BeOfType<OpenCoverParser>();
        }

        [Test]
        public void Parser_UseOpenCoverSpecified_UsesOpenCoverParser()
        {
            var opts = Substitute.For<ICommandOptions>();
            opts.UseOpenCover.Returns(true);
            var coveralls = new CoverallsBootstrap(opts);
            coveralls.CreateParser().Should().BeOfType<OpenCoverParser>();
        }

        [Test]
        public void Parser_ParserOpenCoverSpecified_UsesOpenCoverParser()
        {
            var opts = Substitute.For<ICommandOptions>();
            opts.Parser.ReturnsForAnyArgs(ParserType.OpenCover);
            var coveralls = new CoverallsBootstrap(opts);
            coveralls.CreateParser().Should().BeOfType<OpenCoverParser>();
        }

        [Test]
        public void Parser_ParserTypeUnknown_FailsToInitialize()
        {
            var opts = Substitute.For<ICommandOptions>();
            opts.Parser.ReturnsForAnyArgs(ParserType.Unknown);
            
            Assert.Throws<ArgumentException>(() =>
            {
                var coveralls = new CoverallsBootstrap(opts);
            });
        }

        [Test]
        public void ServiceName_NoEnvironmentVariable_IsCoverallsNet()
        {
            var opts = Substitute.For<ICommandOptions>();
            var coveralls = new CoverallsBootstrap(opts);
            coveralls.ServiceName.Should().Be("coveralls.net");
        }

        [Test]
        public void ServiceName_AppVeyorEnvironment_IsAppveyor()
        {
            Environment.SetEnvironmentVariable("APPVEYOR", "True");

            var opts = Substitute.For<ICommandOptions>();
            var coveralls = new CoverallsBootstrap(opts);

            coveralls.ServiceName.Should().Be("appveyor");
        }

        [Test]
        public void ServiceJobId_OnLocal_IsZero()
        {
            var opts = Substitute.For<ICommandOptions>();
            var coveralls = new CoverallsBootstrap(opts);

            coveralls.ServiceJobId.Should().Be("0");
        }

        [Test]
        public void ServiceJobId_OnAppVeyor_ReadsEnvVariable()
        {
            Environment.SetEnvironmentVariable("APPVEYOR", "True");
            Environment.SetEnvironmentVariable("APPVEYOR_JOB_ID", "23");

            var opts = Substitute.For<ICommandOptions>();
            var coveralls = new CoverallsBootstrap(opts);

            coveralls.ServiceJobId.Should().Be("23");
        }

        [Test]
        public void ServiceJobId_OnJenkins_ReadsEnvVariable()
        {
            Environment.SetEnvironmentVariable("JENKINS_HOME", "True");
            Environment.SetEnvironmentVariable("BUILD_NUMBER", "23");

            var opts = Substitute.For<ICommandOptions>();
            var coveralls = new CoverallsBootstrap(opts);

            coveralls.ServiceJobId.Should().Be("23");
        }

        [Test]
        public void Repository_OnLocal_IsCorrectType()
        {
            var opts = Substitute.For<ICommandOptions>();
            var coveralls = new CoverallsBootstrap(opts);

            coveralls.Repository.Should().BeOfType<LocalGit>();
        }

        [Test]
        public void Repository_OnAppVeyor_IsCorrectType()
        {
            Environment.SetEnvironmentVariable("APPVEYOR", "True");

            var opts = Substitute.For<ICommandOptions>();
            var coveralls = new CoverallsBootstrap(opts);

            coveralls.Repository.Should().BeOfType<AppVeyorGit>();
        }

        [Test]
        public void Repository_OnJenkins_IsCorrectType()
        {
            Environment.SetEnvironmentVariable("JENKINS_HOME", "True");

            var opts = Substitute.For<ICommandOptions>();
            var coveralls = new CoverallsBootstrap(opts);

            coveralls.Repository.Should().BeOfType<JenkinsGit>();
        }

        [Test]
        public void CoverageFiles_NoReportFile_ThrowsException()
        {
            var fileSystem = Substitute.For<IFileSystem>();
            fileSystem.ReadFileText("").ReturnsForAnyArgs(a => null);

            var opts = Substitute.For<ICommandOptions>();
            var coveralls = new CoverallsBootstrap(opts);
            coveralls.FileSystem = fileSystem;

            coveralls.CoverageFiles.Should().BeNull();
        }

        [Test]
        public void CoverageFiles_EmptyReportFile_CoverageFilesIsNull()
        {
            var fileSystem = Substitute.For<IFileSystem>();
            fileSystem.ReadFileText("").ReturnsForAnyArgs("");

            var opts = Substitute.For<ICommandOptions>();
            var coveralls = new CoverallsBootstrap(opts);
            coveralls.FileSystem = fileSystem;

            coveralls.CoverageFiles.Should().BeNull();
        }

        [Test]
        public void CoverageFiles_ValidFile_ReturnsCorrectCountOfFiles()
        {
            var fileSystem = Substitute.For<IFileSystem>();
            fileSystem.ReadFileText("").ReturnsForAnyArgs(TestHelpers.LoadResourceText("Coveralls.Tests.Files.OpenCover.SingleFileCoverage.xml"));

            var opts = Substitute.For<ICommandOptions>();
            opts.InputFiles.Returns(new List<string> {"coverage.xml"});
            var coveralls = new CoverallsBootstrap(opts);
            coveralls.FileSystem = fileSystem;

            coveralls.CoverageFiles.Count().Should().Be(1);
        }

        [Test]
        public void RepoToken_EnvVarNotSet_IsNull()
        {
            var opts = Substitute.For<ICommandOptions>();
            var coveralls = new CoverallsBootstrap(opts);

            coveralls.RepoToken.Should().Be(null);
        }

        [Test]
        public void RepoToken_ReadsEnvVariable()
        {
            Environment.SetEnvironmentVariable("COVERALLS_REPO_TOKEN", "1234abcd");
            var opts = Substitute.For<ICommandOptions>();
            var coveralls = new CoverallsBootstrap(opts);

            coveralls.RepoToken.Should().Be("1234abcd");
        }
    }
}
