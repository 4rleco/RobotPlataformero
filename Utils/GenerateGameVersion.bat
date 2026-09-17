@echo off

cd /d "%~dp0"

for /f "delims=" %%i in ('git tag --sort=-version:refname') do (
    set TAG=%%i
    goto found
)

:found

if "%TAG%"=="" (
    set TAG=dev
)

echo public static class Version > "..\Assets\Scripts\Version.cs"
echo { >> "..\Assets\Scripts\Version.cs"
echo     private const string versionNumber = "%TAG%"; >> "..\Assets\Scripts\Version.cs"
echo. >> "..\Assets\Scripts\Version.cs"
echo     public static string GetVersionNumber() >> "..\Assets\Scripts\Version.cs"
echo     { >> "..\Assets\Scripts\Version.cs"
echo         return versionNumber; >> "..\Assets\Scripts\Version.cs"
echo     } >> "..\Assets\Scripts\Version.cs"
echo } >> "..\Assets\Scripts\Version.cs"