using System;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using Coveralls;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace Coveralls.Tests
{
    [TestFixture]
    public class LocalGitTests
    {
        [Test]
        public void Branches_ShouldContainMaster()
        {
            var fileSystem = Substitute.For<IFileSystem>();
            fileSystem.GetCurrentDirectory().Returns(Directory.GetCurrentDirectory());

            var git = new LocalGit(fileSystem);
            git.Branches.Should().Contain("master");
        }
    }
}