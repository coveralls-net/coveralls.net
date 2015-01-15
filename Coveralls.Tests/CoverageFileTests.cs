using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coveralls.Lib;
using FluentAssertions;
using NUnit.Framework;

namespace Coveralls.Tests
{
    [TestFixture]
    public class CoverageFileTests
    {
        [Test]
        public void Source_SetToGet_ConvertsToUnixLineBreaks()
        {
            var sourceText = "Line1\r\nLine2\r\nLine3";
            var coverageFile = new CoverageFile();

            coverageFile.Source = sourceText;

            coverageFile.Source.Should().Be("Line1\nLine2\nLine3");
        }

        [Test]
        public void Source_SetToGet_MaintainsToUnixLineBreaks()
        {
            var sourceText = "Line1\nLine2\nLine3";
            var coverageFile = new CoverageFile();

            coverageFile.Source = sourceText;

            coverageFile.Source.Should().Be("Line1\nLine2\nLine3");
        }

        [Test]
        public void Coverage_NoLinesCovered_StillHasCorrectLength()
        {
            var sourceText = "Line1\nLine2\nLine3";
            var coverageFile = new CoverageFile();

            coverageFile.Source = sourceText;

            coverageFile.Coverage.Length.Should().Be(3);
        }

        [Test]
        public void Coverage_NoLinesCovered_AllValuesNull()
        {
            var sourceText = "Line1\nLine2\nLine3";
            var coverageFile = new CoverageFile();

            coverageFile.Source = sourceText;

            coverageFile.Coverage[0].Should().Be(null);
            coverageFile.Coverage[1].Should().Be(null);
            coverageFile.Coverage[2].Should().Be(null);
        }

        [Test]
        public void Coverage_OneLineCovered_AllValuesCorrect()
        {
            var sourceText = "Line1\nLine2\nLine3";
            var coverageFile = new CoverageFile();

            coverageFile.Source = sourceText;

            coverageFile.Record(1, 1);

            coverageFile.Coverage[0].Should().Be(1);
            coverageFile.Coverage[1].Should().Be(null);
            coverageFile.Coverage[2].Should().Be(null);
        }

        [Test]
        public void Coverage_MultipleLinesCovered_AllValuesCorrect()
        {
            var sourceText = "Line1\nLine2\nLine3";
            var coverageFile = new CoverageFile();

            coverageFile.Source = sourceText;

            coverageFile.Record(1, 1);
            coverageFile.Record(3, 3);

            coverageFile.Coverage[0].Should().Be(1);
            coverageFile.Coverage[1].Should().Be(null);
            coverageFile.Coverage[2].Should().Be(3);
        }

        [Test]
        public void Coverage_NoSource_LengthIsLargestLineRecorded()
        {
            var coverageFile = new CoverageFile();

            coverageFile.Record(20, 3);

            coverageFile.Coverage.Length.Should().Be(20);
        }
    }
}
