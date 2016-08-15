using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Coveralls.Tests
{
    [TestFixture]
    public class GitDataSerializationTests
    {
        [Test]
        public void Deserialize_ValidJson_UriIsCorrect()
        {
            var data = @"{
                ""name"" : ""origin"",
                ""url"" : ""https://github.com/jdeering/coveralls.net.git"",
            }";

            var remotes = JsonConvert.DeserializeObject<GitRemotes>(data);
            remotes.Uri.Should().Be("https://github.com/jdeering/coveralls.net.git");
        }

        [Test]
        public void Deserialize_BlankUrl_UriIsNull()
        {
            var data = @"{
                ""name"" : ""origin"",
                ""url"" : """",
            }";

            var remotes = JsonConvert.DeserializeObject<GitRemotes>(data);
            remotes.Uri.Should().BeNull();
        }

        [Test]
        public void Deserialize_RelativeUrlFormat_UriIsCorrect()
        {
            var data = @"{
                ""name"" : ""origin"",
                ""url"" : ""/jdeering/coveralls.net.git"",
            }";

            var remotes = JsonConvert.DeserializeObject<GitRemotes>(data);
            remotes.Uri.Should().Be("/jdeering/coveralls.net.git");
        }
        

        [Test]
        public void Serialize_RelativeUrlFormat_UriIsCorrect()
        {
            var data = @"{
                ""name"" : ""origin"",
                ""url"" : ""/jdeering/coveralls.net.git"",
            }";

            var remotes = JsonConvert.DeserializeObject<GitRemotes>(data);
            remotes.Uri.Should().Be("/jdeering/coveralls.net.git");
        }
        

        [Test]
        public void Serialize_JsonStringIsCorrect()
        {
            var remote = new GitRemotes();
            remote.Name = "origin";
            remote.Uri = new System.Uri("/jdeering/coveralls.net.git", System.UriKind.Relative);
            
            var data = JsonConvert.SerializeObject(remote);
            data.Should().Be(@"{""name"":""origin"",""url"":""/jdeering/coveralls.net.git""}");
        }
    }
}