using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using NUnit.Framework;

namespace Coveralls.Tests
{
    public static class TestHelpers
    {
        public static XDocument LoadResourceXml(string resourceName)
        {
            return XDocument.Parse(LoadResourceText(resourceName));
        }

        public static string LoadResourceText(string resourceName)
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            var a = executingAssembly.GetManifestResourceNames();
            using (var stream = executingAssembly.GetManifestResourceStream(resourceName))
            {
                Assert.NotNull(stream, "unable to find resource: " + resourceName + ", within: " + string.Join(", ", a.ToList()));
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}