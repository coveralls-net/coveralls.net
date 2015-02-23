using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Coveralls
{
    [ExcludeFromCodeCoverage]
    public abstract class GitRepository : IGitRepository
    {
        public abstract IEnumerable<string> Branches { get; }
        public abstract string CurrentBranch { get; }
        public abstract IEnumerable<CommitData> Commits { get; }
        public abstract CommitData Head { get; }

        private GitData _data;
        public virtual GitData Data
        {
            get
            {
                if (_data == null)
                {
                    _data = new GitData
                    {
                        Branch = CurrentBranch,
                        Head = Head
                    };
                }

                return _data;
            }
        }

        public virtual void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}