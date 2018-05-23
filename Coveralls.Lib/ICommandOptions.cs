using System.Collections.Generic;

namespace Coveralls
{
    public interface ICommandOptions
    {
        IEnumerable<string> InputFiles { get; }
        ParserType Parser { get; }
        bool UseOpenCover { get; }
        string CoverallsRepoToken { get; }
        bool SendFullSources { get; }
        bool UseCobertura { get; }
    }
}