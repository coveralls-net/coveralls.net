using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

namespace Coveralls
{
    internal enum ServiceType
    {
        AppVeyor,
        Jenkins,
        Unknown
    }

    public class CoverallsBootstrap : IDisposable
    {
        private ICommandOptions _opts;
        private ServiceType _service;

        public CoverallsBootstrap(ICommandOptions options)
        {
            if (options == null) throw new ArgumentException("Invalid command line options.", "options");
            if (options.Parser == ParserType.Unknown) throw new ArgumentException("Unknown parser specified.");

            _opts = options;

            if (Environment.GetEnvironmentVariable("APPVEYOR").IsNotBlank())
                _service = ServiceType.AppVeyor;
            else if (Environment.GetEnvironmentVariable("JENKINS_HOME").IsNotBlank())
                _service = ServiceType.Jenkins;
            else
                _service = ServiceType.Unknown;
        }

        public void Dispose()
        {
            if(_repository != null) _repository.Dispose();
            GC.SuppressFinalize(this);
        }

        public IFileSystem FileSystem { get; set; }

        public string ServiceName
        {
            get
            {
                switch (_service)
                {
                    case ServiceType.AppVeyor:
                        return "appveyor";
                    case ServiceType.Jenkins:
                        return "jenkins";
                    default:
                        return "coveralls.net";
                }
            }
        }

        public string ServiceJobId
        {
            get
            {
                switch (_service)
                {
                    case ServiceType.AppVeyor:
                        return Environment.GetEnvironmentVariable("APPVEYOR_JOB_ID");
                    case ServiceType.Jenkins:
                        return Environment.GetEnvironmentVariable("BUILD_NUMBER");
                    default:
                        return "0";
                }
            }
        }

        private string _repoToken;
        public string RepoToken
        {
            get
            {
                _repoToken = _opts.CoverallsRepoToken;
                if (_repoToken.IsBlank())
                {
                    _repoToken = Environment.GetEnvironmentVariable("COVERALLS_REPO_TOKEN");
                }
                return _repoToken;
            }
        }


        private IEnumerable<CoverageFile> _files;
        public IEnumerable<CoverageFile> CoverageFiles
        {
            get
            {
                if (_files == null || !_files.Any())
                {
                    var coverageFileList = new List<CoverageFile>();

                    foreach (var inputFile in _opts.InputFiles)
                    {
                        var parser = CreateParser();
                        var reportXml = FileSystem.ReadFileText(inputFile);
                        if (reportXml.IsNotBlank())
                        {
                            parser.Report = XDocument.Parse(reportXml);
                            coverageFileList.AddRange(parser.Generate());
                        }
                    }

                    if (_opts.SendFullSources)
                    {
                        foreach (var coverageFile in coverageFileList)
                        {
                            coverageFile.Source = FileSystem.ReadFileText(coverageFile.Path);
                        }
                    }
                    else
                    {
                        foreach (var coverageFile in coverageFileList)
                        {
                            var hash = FileSystem.ComputeHash(coverageFile.Path);

                            var builder = new StringBuilder();
                            foreach (var b in hash)
                            {
                                builder.Append(b.ToString("x2"));
                            }
                            coverageFile.SourceDigest = builder.ToString();
                        }
                    }

                    _files = coverageFileList;
                }

                return _files;
            }
        }

        private IGitRepository _repository;
        public IGitRepository Repository
        {
            get
            {
                if (_repository == null)
                {
                    switch (_service)
                    {
                        case ServiceType.AppVeyor:
                            _repository = new AppVeyorGit();
                            break;
                        case ServiceType.Jenkins:
                            // Jenkins doesn't provide data about the commit in the environment.
                            _repository = new JenkinsGit(FileSystem);
                            break;
                        default:
                            _repository = new LocalGit(FileSystem);
                            break;
                    }
                }
                return _repository;
            }

            set
            {
                _repository = value;
            }
        }

        public ICoverageParser CreateParser()
        {
            switch (_opts.Parser)
            {
                case ParserType.OpenCover:
                    return new OpenCoverParser();
                case ParserType.Cobertura:
                    return new CoberturaCoverageParser();
                case ParserType.AutoDetect:
                    return new AutoParser();
            }
            return new OpenCoverParser();
        }

        public CoverallsData GetData()
        {
            return new CoverallsData
            {
                ServiceName = this.ServiceName,
                ServiceJobId = this.ServiceJobId,
                RepoToken = this.RepoToken,
                SourceFiles = this.CoverageFiles.ToArray(),
                Git = this.Repository.Data
            };
        }
    }
}
