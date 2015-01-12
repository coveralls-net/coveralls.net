using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Coveralls.Lib;
using Coveralls.Lib.Parser;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace Coveralls.Tests
{
    [TestFixture]
    public class OpenCoverParserTests
    {
        [Test]
        public void EmptyReport_NoResults()
        {
            var fileSystem = Substitute.For<IFileSystem>();
            var parser = new OpenCoverParser(fileSystem) { Report = TestHelpers.LoadResourceXml("Coveralls.Tests.Files.OpenCover.EmptyReport.xml") };

            var results = parser.Generate();

            results.Count.Should().Be(0);
        }

        [Test]
        public void SingleFileReport_OneCoverageFile()
        {
            var fileSystem = Substitute.For<IFileSystem>();
            var parser = new OpenCoverParser(fileSystem) { Report = TestHelpers.LoadResourceXml("Coveralls.Tests.Files.OpenCover.SingleFileCoverage.xml") };

            var results = parser.Generate();

            results.Count.Should().Be(1);
        }

        [Test]
        public void SingleFileReport_CoverageFile_ShouldHaveCorrectFileName()
        {
            var fileSystem = Substitute.For<IFileSystem>();
            var parser = new OpenCoverParser(fileSystem) { Report = TestHelpers.LoadResourceXml("Coveralls.Tests.Files.OpenCover.SingleFileCoverage.xml") };

            var coverageFile = parser.Generate().First();

            Path.GetFileName(coverageFile.Path).Should().Be("Utilities.cs");
        }

        [Test]
        public void SingleFileReport_CoverageFile_LineCountShouldBeCorrect()
        {
            var fileSystem = Substitute.For<IFileSystem>();
            fileSystem.ReadFileText(@"c:\src\SymSharp\Symitar\Utilities.cs")
                .Returns(TestHelpers.LoadResourceText("Coveralls.Tests.Files.Utilities.cs"));
            var parser = new OpenCoverParser(fileSystem) { Report = TestHelpers.LoadResourceXml("Coveralls.Tests.Files.OpenCover.SingleFileCoverage.xml") };

            var coverageFile = parser.Generate().First();

            coverageFile.Source.Split('\n').Length.Should().Be(140);
        }

        [Test]
        public void SingleFileReport_CoverageFile_LineCoverageShouldMatchForSomeLines()
        {
            var fileSystem = Substitute.For<IFileSystem>();
            fileSystem.ReadFileText(@"c:\src\SymSharp\Symitar\Utilities.cs")
                .Returns(TestHelpers.LoadResourceText("Coveralls.Tests.Files.Utilities.cs"));
            var parser = new OpenCoverParser(fileSystem) { Report = TestHelpers.LoadResourceXml("Coveralls.Tests.Files.OpenCover.SingleFileCoverage.xml") };

            var coverageFile = parser.Generate().First();
            coverageFile.Coverage[12].Should().Be(16);
            coverageFile.Coverage[13].Should().Be(16);
            coverageFile.Coverage[22].Should().Be(3);
            coverageFile.Coverage[24].Should().Be(11);
        }
    }
}
