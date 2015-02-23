using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coveralls;
using FluentAssertions;
using NUnit.Framework;

namespace Coveralls.Tests
{
    [TestFixture]
    public class PathToolsTests
    {
        [Test]
        public void ToUnixPath_GivenBlankString_IsStillBlank()
        {
            string path = "";
            path.ToUnixPath().Should().Be("");
        }

        [Test]
        public void ToUnixPath_GivenFullPath_IsCorrect()
        {
            string path = @"C:\Src\Path\Test";
            path.ToUnixPath().Should().Be("C:/Src/Path/Test");
        }

        [Test]
        public void ToUnixPath_GivenRelativePath_IsCorrect()
        {
            string path = @"Path\Test";
            path.ToUnixPath().Should().Be("Path/Test");
        }

        [Test]
        public void ToRelativePath_EmptyPath_Unchanged()
        {
            var path = "";
            var baseFolder = "";

            path.ToRelativePath(baseFolder).Should().Be("");
        }

        [Test]
        public void ToRelativePath_EmptyBaseFolder_Unchanged()
        {
            var path = @"c:\full\path\test";
            var baseFolder = "";

            path.ToRelativePath(baseFolder).Should().Be(@"c:\full\path\test");
        }

        [Test]
        public void ToRelativePath_BaseFolderIsPath_ReturnsBlank()
        {
            var path = @"c:\full\path\test";
            var baseFolder = @"c:\full\path\test";

            path.ToRelativePath(baseFolder).Should().Be(@"");
        }

        [Test]
        public void ToRelativePath_BaseFolderIsBeginningOfPath_ReturnsBlank()
        {
            var path = @"c:\full\path\test";
            var baseFolder = @"c:\full\path\test";

            path.ToRelativePath(baseFolder).Should().Be(@"");
        }

        [Test]
        public void ToRelativePath_BaseFolderStartsPath_ReturnsCurrentSubPathInUnixForm()
        {
            var path = @"c:\full\path\test";
            var baseFolder = @"c:\full\";

            path.ToRelativePath(baseFolder).Should().Be(@"path/test");
        }

        [Test]
        public void ToRelativePath_BaseFolderMissingTrailingSlash_ReturnsCurrentSubPathInUnixForm()
        {
            var path = @"c:\full\path\test";
            var baseFolder = @"c:\full";

            path.ToRelativePath(baseFolder).Should().Be(@"path/test");
        }
    }
}
