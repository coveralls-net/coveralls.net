using System;
using System.Globalization;
using System.IO;

namespace Coveralls
{
    public static class PathTools
    {
        public static string ToUnixPath(this string path)
        {
            if (string.IsNullOrEmpty(path)) return "";

            return path.Replace('\\', '/');
        }

        public static string ToRelativePath(this string fullPath, string baseFolder)
        {
            if (string.IsNullOrEmpty(fullPath)) return fullPath;
            if (string.IsNullOrEmpty(baseFolder)) return fullPath;

            if (fullPath.Equals(baseFolder)) return "";

            var pathUri = new Uri(fullPath);

            if (!baseFolder.EndsWith(Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal))
            {
                baseFolder += Path.DirectorySeparatorChar;
            }
            var folderUri = new Uri(baseFolder);

            var relativeUri = folderUri.MakeRelativeUri(pathUri);
            var relativePath = relativeUri.ToString().Replace('/', Path.DirectorySeparatorChar);

            return Uri.UnescapeDataString(relativePath).ToUnixPath();
        }
    }
}