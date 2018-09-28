using System.IO;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Coveralls.Tests
{
    // https://github.com/jdeering/SymSharp
    // E.g. dotcover analyse /targetExecutable=C:\...\nunit-console.exe /output=normal.xml /reporttype=xml /targetarguments="/fixture Symitar.Tests.UtilitiesTests C:\...\Symitar.Tests.dll" /attributeFilters="NUnit.Framework.TestFixture"

    [TestFixture]
    public class DotCoverParserTests
    {
        [Test]
        public void NullReport_NoResults()
        {
            var parser = new DotCoverParser() { Report = null };

            var results = parser.Generate();

            results.Count().Should().Be(0);
        }

        [Test]
        public void WrongReportType_Exception()
        {
            var parser = new DotCoverParser() { Report = TestHelpers.LoadResourceXml("Coveralls.Tests.Files.DotCover.WrongReportType.xml") };

            parser.Invoking(p => p.Generate()).ShouldThrow<InvalidDataException>();
        }

        [Test]
        public void SingleFileReport_OneCoverageFile()
        {
            var parser = new DotCoverParser() { Report = TestHelpers.LoadResourceXml("Coveralls.Tests.Files.DotCover.SingleFileCoverage.xml") };

            var results = parser.Generate();

            results.Count().Should().Be(1);
        }

        [Test]
        public void SingleFileReport_CoverageFile_ShouldHaveCorrectFileName()
        {
            var parser = new DotCoverParser() { Report = TestHelpers.LoadResourceXml("Coveralls.Tests.Files.DotCover.SingleFileCoverage.xml") };

            var coverageFile = parser.Generate().First();

            Path.GetFileName(coverageFile.Path).Should().Be("Utilities.cs");
        }

        [Test]
        public void SingleFileReport_CoverageFile_LineCoverageShouldMatchForSomeLines()
        {
            var parser = new DotCoverParser() { Report = TestHelpers.LoadResourceXml("Coveralls.Tests.Files.DotCover.SingleFileCoverage.xml") };

            var coverageFile = parser.Generate().First();

            coverageFile.Source = TestHelpers.LoadResourceText("Coveralls.Tests.Files.Utilities.cs");

            var lines = coverageFile.Source.Split(new[] {'\n'});

            lines[11].Trim().Should().Be("{");
            coverageFile.Coverage.ToArray()[11].Should().BeGreaterThan(0);
            lines[12].Trim().Should().Be("if (string.IsNullOrEmpty(date))");
            coverageFile.Coverage.ToArray()[12].Should().BeGreaterThan(0);
            lines[19].Trim().Should().Be("hour = 0;");
            coverageFile.Coverage.ToArray()[19].Should().BeGreaterThan(0);
            lines[24].Trim().Should().Be("while (time.Length < 4)");
            coverageFile.Coverage.ToArray()[24].Should().BeGreaterThan(0);
        }
    }
}
