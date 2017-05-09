using FluentAssertions;
using NUnit.Framework;
using System.Linq;

namespace Coveralls.Tests
{
    public class AutoParserTests
    {
        [Test]
        public void NullReport_NoResults()
        {
            var parser = new AutoParser() { Report = null };

            var results = parser.Generate();

            results.Count().Should().Be(0);
        }

        [Test]
        public void SingleOpenCoverFileReport_CoverageFile_LineCoverageShouldMatchForSomeLines()
        {
            var parser = new AutoParser() { Report = TestHelpers.LoadResourceXml("Coveralls.Tests.Files.OpenCover.SingleFileCoverage.xml") };

            var coverageFile = parser.Generate().First();

            coverageFile.Source = TestHelpers.LoadResourceText("Coveralls.Tests.Files.Utilities.cs");

            var lines = coverageFile.Source.Split(new[] { '\n' });

            lines[11].Trim().Should().Be("{");
            coverageFile.Coverage.ToArray()[11].Should().Be(16);
            lines[12].Trim().Should().Be("if (string.IsNullOrEmpty(date))");
            coverageFile.Coverage.ToArray()[12].Should().Be(16);
            lines[19].Trim().Should().Be("hour = 0;");
            coverageFile.Coverage.ToArray()[19].Should().Be(3);
            lines[24].Trim().Should().Be("while (time.Length < 4)");
            coverageFile.Coverage.ToArray()[24].Should().Be(15);
        }

        [Test]
        public void SingleCoberturaFileReport_CoverageFile_LineCoverageShouldMatchForSomeLines()
        {
            var parser = new AutoParser() { Report = TestHelpers.LoadResourceXml("Coveralls.Tests.Files.Cobertura.SingleFileCoverage.xml") };

            var coverageFile = parser.Generate().First();

            coverageFile.Source = TestHelpers.LoadResourceText("Coveralls.Tests.Files.app.ts");

            var lines = coverageFile.Source.Split(new[] { '\n' });

            lines[0].Trim().Should().Be("angular.module('app', [");
            coverageFile.Coverage.ToArray()[0].Should().Be(1);
            lines[11].Trim().Should().Be("$locationProvider.html5Mode(false);");
            coverageFile.Coverage.ToArray()[11].Should().Be(0);
        }
    }
}
