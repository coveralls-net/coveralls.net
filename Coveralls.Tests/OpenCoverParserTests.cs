using System.IO;
using System.Linq;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace Coveralls.Tests
{
    [TestFixture]
    public class OpenCoverParserTests
    {
        [Test]
        public void NullReport_NoResults()
        {
            var parser = new OpenCoverParser() { Report = null };

            var results = parser.Generate();

            results.Count().Should().Be(0);
        }
        [Test]
        public void EmptyReport_NoResults()
        {
            var parser = new OpenCoverParser() { Report = TestHelpers.LoadResourceXml("Coveralls.Tests.Files.OpenCover.EmptyReport.xml") };

            var results = parser.Generate();

            results.Count().Should().Be(0);
        }

        [Test]
        public void SingleFileReport_OneCoverageFile()
        {
            var parser = new OpenCoverParser() { Report = TestHelpers.LoadResourceXml("Coveralls.Tests.Files.OpenCover.SingleFileCoverage.xml") };

            var results = parser.Generate();

            results.Count().Should().Be(1);
        }

        [Test]
        public void SingleFileReport_CoverageFile_ShouldHaveCorrectFileName()
        {
            var parser = new OpenCoverParser() { Report = TestHelpers.LoadResourceXml("Coveralls.Tests.Files.OpenCover.SingleFileCoverage.xml") };

            var coverageFile = parser.Generate().First();

            Path.GetFileName(coverageFile.Path).Should().Be("Utilities.cs");
        }

        [Test]
        public void SingleFileReport_CoverageFile_LineCoverageShouldMatchForSomeLines()
        {
            var parser = new OpenCoverParser() { Report = TestHelpers.LoadResourceXml("Coveralls.Tests.Files.OpenCover.SingleFileCoverage.xml") };

            var coverageFile = parser.Generate().First();

            coverageFile.Source = TestHelpers.LoadResourceText("Coveralls.Tests.Files.Utilities.cs");

            var lines = coverageFile.Source.Split(new[] {'\n'});

            lines[11].Trim().Should().Be("{");
            coverageFile.Coverage.ToArray()[11].Should().Be(16);
            lines[12].Trim().Should().Be("if (string.IsNullOrEmpty(date))");
            coverageFile.Coverage.ToArray()[12].Should().Be(16);
            lines[19].Trim().Should().Be("hour = 0;");
            coverageFile.Coverage.ToArray()[19].Should().Be(3);
            lines[24].Trim().Should().Be("while (time.Length < 4)");
            coverageFile.Coverage.ToArray()[24].Should().Be(15);
        }
    }
}
