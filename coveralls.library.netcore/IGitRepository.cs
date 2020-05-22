using System;

namespace Coveralls.Library
{
    public interface IGitRepository : IDisposable
    {
        GitData Data { get; }
    }
}