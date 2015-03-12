using System;
using System.Collections.Generic;
using System.Linq;
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
                    List<CoverageFile> allCoverageFiles = new List<CoverageFile>();

                    foreach (string inputFile in _opts.InputFiles)
                    {
                        var parser = CreateParser();
                        var reportXml = FileSystem.ReadFileText(inputFile);
                        if (reportXml.IsNotBlank())
                        {
                            parser.Report = XDocument.Parse(reportXml);
                            allCoverageFiles.AddRange(parser.Generate());
                        }
                    }

                    // If we want the md5 digest and not the full source, loop through the coverage files
                    // and set the option on the class.

                    if (_opts.SendFullSources)
                    {
                        foreach(CoverageFile coverageFile in allCoverageFiles)
                        {
                            coverageFile.Digest = false;
                        }
                    }

                    if(allCoverageFiles.Any())
                    {
                        _files = allCoverageFiles;
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
                        case ServiceType.Jenkins:
                            // Jenkins doesn't provide data about the commit in the environment.
                            _repository = new LocalGit();
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

    }
}
