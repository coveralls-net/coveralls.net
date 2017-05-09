using Coveralls;
using FluentAssertions;
using NUnit.Framework;
using System.IO;
using System.Linq;

namespace Coveralls.Tests
{
    public class CoberturaCoverageParserTests
    {
        [Test]
        public void EmptyReport_NoResults()
        {
        }

        [Test]
        public void NullReport_NoResults()
        {
            var parser = new CoberturaCoverageParser() { Report = null };

            var results = parser.Generate();

            results.Count().Should().Be(0);
        }

        [Test]
        public void SingleFileReport_CoverageFile_LineCoverageShouldMatchForSomeLines()
        {
            var parser = new CoberturaCoverageParser() { Report = TestHelpers.LoadResourceXml("Coveralls.Tests.Files.Cobertura.SingleFileCoverage.xml") };

            var coverageFile = parser.Generate().First();

            coverageFile.Source = TestHelpers.LoadResourceText("Coveralls.Tests.Files.app.ts");

            var lines = coverageFile.Source.Split(new[] { '\n' });

            lines[0].Trim().Should().Be("angular.module('app', [");
            coverageFile.Coverage.ToArray()[0].Should().Be(1);
            lines[11].Trim().Should().Be("$locationProvider.html5Mode(false);");
            coverageFile.Coverage.ToArray()[11].Should().Be(0);
        }

        [Test]
        public void SingleFileReport_CoverageFile_ShouldHaveCorrectFileName()
        {
            var parser = new CoberturaCoverageParser() { Report = TestHelpers.LoadResourceXml("Coveralls.Tests.Files.Cobertura.SingleFileCoverage.xml") };

            var coverageFile = parser.Generate().First();

            Path.GetFileName(coverageFile.Path).Should().Be("app.ts");
        }

        [Test]
        public void SingleFileReport_OneCoverageFile()
        {
            var parser = new CoberturaCoverageParser() { Report = TestHelpers.LoadResourceXml("Coveralls.Tests.Files.Cobertura.SingleFileCoverage.xml") };

            var results = parser.Generate();

            results.Count().Should().Be(1);
        }
    }
}