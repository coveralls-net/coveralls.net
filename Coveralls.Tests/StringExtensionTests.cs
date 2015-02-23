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
    public class StringExtensionTests
    {
        [Test]
        public void IsBlank_NullString_IsTrue()
        {
            string s = null;
            s.IsBlank().Should().BeTrue();
        }

        [Test]
        public void IsBlank_EmptyString_IsTrue()
        {
            string s = string.Empty;
            s.IsBlank().Should().BeTrue();
        }

        [Test]
        public void IsBlank_SingleSpace_IsFalse()
        {
            string s = " ";
            s.IsBlank().Should().BeFalse();
        }

        [Test]
        public void IsBlank_Word_IsFalse()
        {
            string s = "WORD";
            s.IsBlank().Should().BeFalse();
        }

        [Test]
        public void IsNotBlank_NullString_IsFalse()
        {
            string s = null;
            s.IsNotBlank().Should().BeFalse();
        }

        [Test]
        public void IsNotBlank_EmptyString_IsFalse()
        {
            string s = string.Empty;
            s.IsNotBlank().Should().BeFalse();
        }

        [Test]
        public void IsNotBlank_SingleSpace_IsTrue()
        {
            string s = " ";
            s.IsNotBlank().Should().BeTrue();
        }

        [Test]
        public void IsNotBlank_Word_IsTrue()
        {
            string s = "WORD";
            s.IsNotBlank().Should().BeTrue();
        }
    }
}
