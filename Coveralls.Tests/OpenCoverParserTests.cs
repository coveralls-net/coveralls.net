using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
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

    public static class TestHelpers
    {
        public static XDocument LoadResourceXml(string resourceName)
        {
            return XDocument.Parse(LoadResourceText(resourceName));
        }

        public static string LoadResourceText(string resourceName)
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            using (var stream = executingAssembly.GetManifestResourceStream(resourceName))
            {
                Assert.NotNull(stream);
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
