using System;
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
            if (baseFolder[baseFolder.Length - 1] != '\\' && baseFolder[baseFolder.Length - 1] != '/')
                baseFolder += Path.DirectorySeparatorChar;

            var pathUri = new Uri(fullPath, UriKind.Absolute);
            var baseUri = new Uri(baseFolder, UriKind.Absolute);
            return baseUri.MakeRelativeUri(pathUri).ToString();
        }
    }
}