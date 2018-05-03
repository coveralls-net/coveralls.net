using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibGit2Sharp;

namespace Coveralls
{
    public class LocalGit : GitRepository
    {
        private IFileSystem _fileSystem;
        protected Repository _repository;

        public LocalGit(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;

            var workingDirectory = _fileSystem.GetCurrentDirectory();

            var directory = new DirectoryInfo(workingDirectory);
            while (!directory.EnumerateDirectories().Any(x => x.Name == ".git"))
            {
                directory = directory.Parent;
                if (directory == directory.Root) break;
            }

            _repository = new Repository(directory.FullName + "\\.git");
        }

        public sealed override void Dispose()
        {
            _repository.Dispose();
            _repository = null;

            GC.SuppressFinalize(this);
        }

        public override IEnumerable<string> Branches
        {
            get { return _repository.Branches.Select(x => x.Name); }
        }

        public override IEnumerable<CommitData> Commits
        {
            get
            {
                return _repository.Head.Commits.Select(c =>
                    new CommitData()
                    {
                        Id = c.Id.Sha,
                        Message = c.Message,
                        Author = c.Author.Name,
                        AuthorEmail = c.Author.Email,
                        Committer = c.Committer.Name,
                        CommitterEmail = c.Committer.Email
                    });
            }
        }

        public override string CurrentBranch { get { return _repository.Head.Name; } }
        public override CommitData Head
        {
            get { return Commits.First(); } 
        }
    }
}