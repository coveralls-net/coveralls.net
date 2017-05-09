using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
        public void Constructor_NullOptions_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var coveralls = new CoverallsBootstrap(null);
            });
        }

        [Test]
        public void Parser_NoOptionsSpecified_UsesOpenCoverParser()
        {
            var opts = Substitute.For<ICommandOptions>();
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
        public void Parser_ParserCoberturaSpecified_UsesCoberturaParser()
        {
            var opts = Substitute.For<ICommandOptions>();
            opts.Parser.ReturnsForAnyArgs(ParserType.Cobertura);
            var coveralls = new CoverallsBootstrap(opts);
            coveralls.CreateParser().Should().BeOfType<CoberturaCoverageParser>();
        }

        [Test]
        public void Parser_ParserAutoDetectSpecified_UsesAutoParser()
        {
            var opts = Substitute.For<ICommandOptions>();
            opts.Parser.ReturnsForAnyArgs(ParserType.AutoDetect);
            var coveralls = new CoverallsBootstrap(opts);
            coveralls.CreateParser().Should().BeOfType<AutoParser>();
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
            coveralls.FileSystem = Stub.LocalFileSystem();

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
            coveralls.FileSystem = Stub.LocalFileSystem();

            coveralls.Repository.Should().BeOfType<JenkinsGit>();
        }

        [Test]
        public void CoverageFiles_EmptyReportFile_CoverageFilesIsEmpty()
        {
            var fileSystem = Substitute.For<IFileSystem>();
            fileSystem.ReadFileText("").ReturnsForAnyArgs("");

            var opts = Substitute.For<ICommandOptions>();
            var coveralls = new CoverallsBootstrap(opts);
            coveralls.FileSystem = fileSystem;

            coveralls.CoverageFiles.Should().BeEmpty();
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
        public void CoverageFiles_FullSource_LeavesSourceDigestNull()
        {
            var fileSystem = Substitute.For<IFileSystem>();
            fileSystem.ReadFileText("").ReturnsForAnyArgs(TestHelpers.LoadResourceText("Coveralls.Tests.Files.OpenCover.SingleFileCoverage.xml"));
            
            var opts = Substitute.For<ICommandOptions>();
            opts.InputFiles.Returns(new List<string> {"coverage.xml"});
            opts.SendFullSources.Returns(true);
            var coveralls = new CoverallsBootstrap(opts);
            coveralls.FileSystem = fileSystem;

            var coveredFileData = coveralls.CoverageFiles.First();
            coveredFileData.SourceDigest.Should().BeNull();
        }
        
        [Test]
        public void CoverageFiles_FullSource_RetrievesFullFileContents()
        {
            var fileContents = TestHelpers.LoadResourceText("Coveralls.Tests.Files.Utilities.cs");
            var fileContentsUnix = fileContents.Replace("\r\n", "\n");

            var fileSystem = Substitute.For<IFileSystem>();
            fileSystem.ReadFileText("coverage.xml").Returns(TestHelpers.LoadResourceText("Coveralls.Tests.Files.OpenCover.SingleFileCoverage.xml"));
            fileSystem.ReadFileText(@"c:/src/SymSharp/Symitar/Utilities.cs").Returns(fileContents);
            
            var opts = Substitute.For<ICommandOptions>();
            opts.InputFiles.Returns(new List<string> {"coverage.xml"});
            opts.SendFullSources.Returns(true);
            var coveralls = new CoverallsBootstrap(opts);
            coveralls.FileSystem = fileSystem;

            var coveredFileData = coveralls.CoverageFiles.First();
            coveredFileData.Source.Should().Be(fileContentsUnix);
        }
        
        [Test]
        public void CoverageFiles_NotFullSource_SourceDigestNotNull()
        {
            var fileSystem = Substitute.For<IFileSystem>();
            fileSystem.ReadFileText("").ReturnsForAnyArgs(TestHelpers.LoadResourceText("Coveralls.Tests.Files.OpenCover.SingleFileCoverage.xml"));
            
            var opts = Substitute.For<ICommandOptions>();
            opts.InputFiles.Returns(new List<string> {"coverage.xml"});
            opts.SendFullSources.Returns(false);
            var coveralls = new CoverallsBootstrap(opts);
            coveralls.FileSystem = fileSystem;

            var coveredFileData = coveralls.CoverageFiles.First();
            coveredFileData.SourceDigest.Should().NotBeNull();
        }
        
        [Test]
        public void CoverageFiles_NotFullSource_SetsDigestToFileHash()
        {
            var hash = "d131dd02c5e6eec4d131dd02c5e6eec4d131dd02c5e6eec4";
            var bytes = Enumerable.Range(0, hash.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(hash.Substring(x, 2), 16))
                     .ToArray();

            var fileSystem = Substitute.For<IFileSystem>();
            fileSystem.ReadFileText("").ReturnsForAnyArgs(TestHelpers.LoadResourceText("Coveralls.Tests.Files.OpenCover.SingleFileCoverage.xml"));
            fileSystem.ComputeHash("").ReturnsForAnyArgs(bytes);
            
            var opts = Substitute.For<ICommandOptions>();
            opts.InputFiles.Returns(new List<string> {"coverage.xml"});
            opts.SendFullSources.Returns(false);
            var coveralls = new CoverallsBootstrap(opts);
            coveralls.FileSystem = fileSystem;

            var coveredFileData = coveralls.CoverageFiles.First();
            coveredFileData.SourceDigest.Should().Be(hash);
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

        [Test]
        public void Dispose_DisposesOfRepository()
        {
            var gitRepo = Substitute.For<IGitRepository>();

            var opts = Substitute.For<ICommandOptions>();
            var coveralls = new CoverallsBootstrap(opts);
            coveralls.Repository = gitRepo;

            coveralls.Dispose();

            gitRepo.Received().Dispose();
        }

        [Test]
        public void GetData_ReturnsCorrectDataBasedOnEnvironment()
        {
            Environment.SetEnvironmentVariable("COVERALLS_REPO_TOKEN", "1234abcd");
            Environment.SetEnvironmentVariable("JENKINS_HOME", "True");
            Environment.SetEnvironmentVariable("BUILD_NUMBER", "23");

            var opts = Substitute.For<ICommandOptions>();
            var coveralls = new CoverallsBootstrap(opts);
            coveralls.FileSystem = Stub.LocalFileSystem();

            var data = coveralls.GetData();
            data.ServiceName.Should().Be("jenkins");
            data.ServiceJobId.Should().Be("23");
            data.RepoToken.Should().Be("1234abcd");
        }
    }

    public static class Stub
    {
        public static IFileSystem LocalFileSystem()
        {
            var fileSystem = Substitute.For<IFileSystem>();
            fileSystem.GetCurrentDirectory().Returns(Directory.GetCurrentDirectory());

            return fileSystem;
        }
    }
}
