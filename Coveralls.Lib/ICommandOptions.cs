namespace Coveralls
{
    public interface ICommandOptions
    {
        string InputFile { get; }
        ParserType Parser { get; }
        bool UseOpenCover { get; }
        bool DebugMode { get; }
    }
}