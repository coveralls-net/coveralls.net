using System;

namespace Coveralls
{
    public interface IGitRepository : IDisposable
    {
        GitData Data { get; }
    }
}