
$APPVEYOR = [Environment]::GetEnvironmentVariable("APPVEYOR")
$APPVEYOR_BUILD_VERSION = [Environment]::GetEnvironmentVariable("APPVEYOR_BUILD_VERSION")

if($APPVEYOR) {
	cd coveralls.net\package
	nuget pack coveralls.net.nuspec -Version "$APPVEYOR_BUILD_VERSION"
}