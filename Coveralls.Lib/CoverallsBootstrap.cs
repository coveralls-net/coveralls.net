using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Coveralls
{
    internal enum ServiceType
    {
        AppVeyor,
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

            _service = Environment.GetEnvironmentVariable("APPVEYOR").IsNotBlank() ? ServiceType.AppVeyor : ServiceType.Unknown;
        }

        public void Dispose()
        {
            _repository.Dispose();
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
                if (_repoToken.IsBlank())
                {
                    _repoToken = Environment.GetEnvironmentVariable("COVERALLS_REPO_TOKEN");
                }

                return _repoToken;
            }
            set { _repoToken = value; }
        }

        private IEnumerable<CoverageFile> _files;
        public IEnumerable<CoverageFile> CoverageFiles
        {
            get
            {
                if (_files == null || !_files.Any())
                {
                    var parser = CreateParser();
                    var reportXml = FileSystem.ReadFileText(_opts.InputFile);
                    if (reportXml.IsNotBlank())
                    {
                        parser.Report = XDocument.Parse(reportXml);
                        _files = parser.Generate();
                    }
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
                        default:
                            _repository = new LocalGit();
                            break;
                    }
                }
                return _repository;
            }
        }

        public ICoverageParser CreateParser()
        {
            switch (_opts.Parser)
            {
                case ParserType.OpenCover:
                    return new OpenCoverParser(FileSystem);
            }
            return new OpenCoverParser(FileSystem);
        }

        public IGitRepository CreateGitRepository()
        {
            if (Environment.GetEnvironmentVariable("APPVEYOR_JOB_ID").IsNotBlank()) _repository = new AppVeyorGit();
            else _repository = new LocalGit();

            return _repository;
        }
    }
}
