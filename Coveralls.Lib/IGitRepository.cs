using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibGit2Sharp;

namespace Coveralls.Lib
{
    public interface IGitRepository
    {
        GitData Data { get; }
    }

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
    }

    public class LocalGit : GitRepository
    {
        private Repository _repository;
        public LocalGit()
        {
            var workingDirectory = Directory.GetCurrentDirectory();

            var directory = new DirectoryInfo(workingDirectory);
            while (!directory.EnumerateDirectories().Any(x => x.Name == ".git"))
            {
                directory = directory.Parent;
                if (directory == directory.Root) break;
            }

            _repository = new Repository(directory.FullName + "\\.git");
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
                        AuthorEmail = c.Author.Email
                    });
            }
        }

        public override string CurrentBranch { get { return _repository.Head.Name; } }
        public override CommitData Head
        {
            get { return Commits.First(); } 
        }
    }

    public class AppVeyorGit : GitRepository
    {
        public override IEnumerable<string> Branches { get { return new List<string>() { CurrentBranch }; } }
        public override IEnumerable<CommitData> Commits { get { return new List<CommitData>() { Head }; } }

        private string _branch;
        public override string CurrentBranch 
        { 
            get
            {
                if(string.IsNullOrEmpty(_branch)) _branch = Environment.GetEnvironmentVariable("APPVEYOR_REPO_BRANCH");
                return _branch;
            } 
        }

        private CommitData _head;
        public override CommitData Head
        {
            get
            {
                if (_head == null)
                {
                    _head = new CommitData
                    {
                        Id = Environment.GetEnvironmentVariable("APPVEYOR_REPO_COMMIT"),
                        Message = Environment.GetEnvironmentVariable("APPVEYOR_REPO_COMMIT_MESSAGE"),
                        Author = Environment.GetEnvironmentVariable("APPVEYOR_REPO_COMMIT_AUTHOR"),
                        AuthorEmail = Environment.GetEnvironmentVariable("APPVEYOR_REPO_COMMIT_AUTHOREMAIL"),
                        PullRequestId = Environment.GetEnvironmentVariable("APPVEYOR_PULL_REQUEST_NUMBER")
                    };
                }
                return _head;
            }
        }
    }
}