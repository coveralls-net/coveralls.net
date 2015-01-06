using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Coveralls.Lib;
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
            var parser = new OpenCoverParser(fileSystem) { Report = TestHelpers.LoadResourceXml("Coveralls.Tests.Files.EmptyReport.xml") };

            var results = parser.Generate();

            results.Count.Should().Be(0);
        }

        [Test]
        public void SingleFileReport_OneCoverageFile()
        {
            var fileSystem = Substitute.For<IFileSystem>();
            var parser = new OpenCoverParser(fileSystem) { Report = TestHelpers.LoadResourceXml("Coveralls.Tests.Files.SingleFileCoverage.xml") };

            var results = parser.Generate();

            results.Count.Should().Be(1);
        }
    }
}
