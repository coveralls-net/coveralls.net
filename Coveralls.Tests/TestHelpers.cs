using System.IO;
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
            using (var stream = executingAssembly.GetManifestResourceStream(resourceName))
            {
                Assert.NotNull(stream);
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}