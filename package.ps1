
$APPVEYOR = [Environment]::GetEnvironmentVariable("APPVEYOR")
$APPVEYOR_BUILD_VERSION = [Environment]::GetEnvironmentVariable("APPVEYOR_BUILD_VERSION")

if($APPVEYOR) {
	# move to package directory
	cd coveralls.net\package
	
	# package with build/version number
	nuget pack coveralls.net.nuspec -Version "$APPVEYOR_BUILD_VERSION"

	# return to root
	cd ..\..
}