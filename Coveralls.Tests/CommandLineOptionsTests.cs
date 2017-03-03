using Coveralls.Net;
using FluentAssertions;
using NUnit.Framework;

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
        public void CommandLineOptions_UseCoberturaSpecified_SetsParserToCobertura()
        {
            var opts = new CommandLineOptions();
            opts.UseCobertura = true;
            opts.Parser.Should().Be(ParserType.Cobertura);
        }

        [Test]
        public void CommandLineOptions_UseAutoDetectSpecified_SetsParserToAutoDetect()
        {
            var opts = new CommandLineOptions();
            opts.UseAutoDetect = true;
            opts.Parser.Should().Be(ParserType.AutoDetect);
        }
    }
}
