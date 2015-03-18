using System;
using System.Collections.Generic;

namespace Coveralls
{
    public class JenkinsGit : LocalGit
    {
        public override IEnumerable<string> Branches { get { return new List<string>() { CurrentBranch }; } }

        private string _branch;
        public override string CurrentBranch
        {
            get
            {
                if (string.IsNullOrEmpty(_branch)) _branch = Environment.GetEnvironmentVariable("GIT_BRANCH");

                // For github...
                // The git branch is of the form origin/<branch> or origin/pr/1152/merge.  The links on the coveralls page
                // will link to that branch, but only the first form (without origin) will work.  Strip away all but the last
                // element for the first cast and leave everything for the first one.

                if (_branch.Contains("/")) {
                    if (!(_branch.Contains("pr") || _branch.Contains("/merge"))) {
                        int index = _branch.IndexOf("/") + 1;
                        if(index < _branch.Length){
                            _branch = _branch.Substring(index);
                        }
                    }
                }

                return _branch;
            }
        }
    }
}
