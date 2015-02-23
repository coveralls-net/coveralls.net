namespace Coveralls.Lib
{
    public interface ICommandOptions
    {
        string InputFile { get; }
        ParserTypes Parser { get; }
        bool UseOpenCover { get; }
        bool DebugMode { get; }
    }
}