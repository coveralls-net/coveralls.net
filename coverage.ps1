
$APPVEYOR = [Environment]::GetEnvironmentVariable("APPVEYOR")
$CONFIGURATION = [Environment]::GetEnvironmentVariable("CONFIGURATION")

function Generate-Coverage-Report {
	if($CONFIGURATION) {
		Write-Host "Generating coverage report ..."
		$filter = "+[Coveralls*]*"
		$args = "/noshadow Coveralls.Tests\bin\$CONFIGURATION\Coveralls.Tests.dll /domain:single"
		packages\OpenCover.4.5.3522\OpenCover.Console.exe -register:user -filter:"$filter" -hideskipped:All -target:"nunit-console.exe" -targetargs:"$args" -output:coverage.xml
		return true
	}

	return false
}

function Run-Coveralls {
	packages\coveralls.io.1.1.32\tools\coveralls.net.exe --opencover coverage.xml
}

if($env:APPVEYOR) {
	if(Generate-Coverage-Report) {
		Write-Host "Sending report to coveralls.io ..."
		Run-Coveralls
	}
}