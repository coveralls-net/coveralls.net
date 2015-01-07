
[Environment]::SetEnvironmentVariable("APPVEYOR", "true")
[Environment]::SetEnvironmentVariable("CONFIGURATION", "Debug")

$APPVEYOR = [Environment]::GetEnvironmentVariable("APPVEYOR")
$CONFIGURATION = [Environment]::GetEnvironmentVariable("CONFIGURATION")

function Generate-Coverage-Report {
	if($CONFIGURATION) {
		Write-Host "Generating coverage report ..."
		$filter = "+[Coveralls*]*"
		$opencover = ""
		$nunit = "packages\NUnit.Runners.2.6.4\tools\nunit-console.exe"
		$nunitArgs = "/noshadow /domain:single Coveralls.Tests\bin\$CONFIGURATION\Coveralls.Tests.dll"
				
		$cmd = @"
packages\OpenCover.4.5.3522\OpenCover.Console.exe -register:user -target:"$nunit" -targetargs:"$nunitArgs" -output:coverage.xml
"@
		Write-Host "Running: $cmd"
		Invoke-Expression -Command:$cmd

		return $true
	}

	return $false
}

function Run-Coveralls {
	packages\coveralls.io.1.1.50\tools\coveralls.net.exe --opencover coverage.xml
}

if($env:APPVEYOR) {
	if(Generate-Coverage-Report) {
		Write-Host "Creating artifact ..."
		Push-AppveyorArtifact coverage.xml

		Write-Host "Sending report to coveralls.io ..."
		Run-Coveralls
	}
}