using System;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using Coveralls;
using FluentAssertions;
using NUnit.Framework;

namespace Coveralls.Tests
{
    [TestFixture]
    public class LocalGitTests
    {
        [Test]
        public void Branches_ShouldContainMaster()
        {
            var git = new LocalGit();
            git.Branches.Should().Contain("master");
        }
    }
}