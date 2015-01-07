coveralls.net
=============

[![Build status](https://ci.appveyor.com/api/projects/status/yxp5vpnesuji5pbd/branch/master?svg=true)](https://ci.appveyor.com/project/jdeering/coveralls-net/branch/master)
[![Coverage Status](https://img.shields.io/coveralls/jdeering/coveralls.net.svg)](https://coveralls.io/r/jdeering/coveralls.net)
[![Code Quality](https://scan.coverity.com/projects/3856/badge.svg)](https://scan.coverity.com/projects/3856)

Coveralls uploader for .Net code coverage reporting


## Planned Support

These are the targets, but I'm not sure how far I will be able to get on my own. I am currently only working towards
support for OpenCover as that is currently my framework of choice.

* [OpenCover](https://github.com/sawilde/opencover)
* [NCover](https://www.ncover.com/)
* [Visual Studio Coverage](http://msdn.microsoft.com/en-us/library/dd299398%28v=vs.90%29.aspx)
* [Mono Code Coverage (monocov)](http://www.mono-project.com/docs/debug+profile/profile/code-coverage/)
* [SharpCover](https://github.com/gaillard/SharpCover)


## Versioning

Visor will be maintained under the Semantic Versioning guidelines as much as possible. Releases will be numbered with the following format:

`<major>.<minor>.<patch>`

and constructed with the following guidelines:

* Breaking backward compatibility bumps the major (resetting the minor and patch)
* New additions without breaking backward compatibility bumps the minor (resetting the patch)
* Bug fixes and misc changes bump the patch

For more information on SemVer, please visit http://semver.org/.


## Bug tracker

Have a bug or a feature request? [Please open a new issue](https://github.com/jdeering/coveralls.net/issues). Before opening a new issue, please search for existing issues.


## Contributing

Please submit all pull requests against WIP branches. You must include all relevant unit tests for any bug fixes or new features.

Thanks!


## Authors

**Jason Deering**

+ http://twitter.com/jasondeering
+ http://github.com/jdeering

## License

Licensed under the GNU General Public License, Version 3.0 (the "License"); you may not use this work except in compliance with the License. You may obtain a copy of the License at:

* http://www.gnu.org/licenses/gpl-3.0.txt

Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.