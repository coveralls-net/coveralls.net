using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coveralls.Lib;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace Coveralls.Tests
{
    [TestFixture]
    public class CoverallsBootstrapTests
    {
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
            opts.Parser.ReturnsForAnyArgs(ParserTypes.OpenCover);
            var coveralls = new CoverallsBootstrap(opts);
            coveralls.CreateParser().Should().BeOfType<OpenCoverParser>();
        }

        [Test]
        public void Parser_ParserTypeUnknown_FailsToInitialize()
        {
            var opts = Substitute.For<ICommandOptions>();
            opts.Parser.ReturnsForAnyArgs(ParserTypes.Unknown);
            
            Assert.Throws<ArgumentException>(() =>
            {
                var coveralls = new CoverallsBootstrap(opts);
            });
        }
    }
}
