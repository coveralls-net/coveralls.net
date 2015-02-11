using System.Collections.Generic;

namespace Coveralls.Lib
{
    public interface ICommandOptions
    {
        IEnumerable<string> InputFiles { get; }
        ParserTypes Parser { get; }
        bool UseOpenCover { get; }
        string CoverallsRepoToken { get; }
    }
}