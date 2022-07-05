$Env:GOARCH = "amd64"
$Env:GOOS = "linux"
$Env:CGO_ENABLED = "0"


function build {
    $current = Get-Location
    Remove-Item -Force -Recurse -ErrorAction SilentlyContinue ./build
    New-Item -ItemType Directory ./build
    Set-Location ./build
    New-Item -ItemType Directory ./lobby
    New-Item -ItemType Directory ./match
    New-Item -ItemType Directory ./game

    Set-Location $current/lobby/run
    go build -o ../../build/lobby/lobby 

    Set-Location $current/match/run
    go build -o ../../build/match/match 

    Set-Location $current/game/run
    go build -o ../../build/game/game 
`
    Set-Location $current
}

function clean {
    Remove-Item -Force -Recurse -ErrorAction SilentlyContinue ./build
    Write-Output "rm ./build"
}

$cmd = $args[0]
switch ($cmd) {
    "build" {
        build
    }
    "clean" {
        clean
    }
    Default {
        Write-Output "useage: ./build.ps1 [build|clean]"
    }
}
