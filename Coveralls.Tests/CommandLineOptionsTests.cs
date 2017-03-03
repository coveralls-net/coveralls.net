using Coveralls.Net;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coveralls.Tests
{
    public class CommandLineOptionsTests
    {
        [Test]
        public void CommandLineOptions_UseOpenCoverSpecified_SetsParserToOpenCover()
        {
            var opts = new CommandLineOptions();
            opts.UseOpenCover = true;
            opts.Parser.Should().Be(ParserType.OpenCover);
        }

        [Test]
        public void CommandLineOptions_UseCoberturaSpecified_SetsParserCobertura()
        {
            var opts = new CommandLineOptions();
            opts.UseCobertura = true;
            opts.Parser.Should().Be(ParserType.Cobertura);
        }
    }
}
