REM Create a 'Releases' folder if it does not exist
if not exist "%~dp0Releases" mkdir "%~dp0Releases"
if not exist "%~dp0Releases\Staging" mkdir "%~dp0Releases\Staging"

REM Delete existing temp directory if it exists
if EXIST "%~dp0Releases\Build" del "%~dp0Releases\Build"

call :BuildRelease

if %errorlevel% equ 0 (
	call :CopyToBuildDirectory
)

if %errorlevel% equ 0 (
	call :CreateReleasePackage
)

if EXIST "%~dp0Releases\Staging" rmdir /s /q "%~dp0Releases\Staging"

exit /b %errorlevel%

:BuildRelease
"C:\Program Files (x86)\MSBuild\14.0\Bin\msbuild.exe" "CodeMovement.EbcdicCompare.sln" ^
/t:Build ^
/p:Configuration="Release"
exit /b %errorlevel%

:CopyToBuildDirectory
xcopy "%~dp0CodeMovement.EbcdicCompare.App\bin\Release" "%~dp0Releases\Staging" /e /s /y
exit /b %errorlevel%

:CreateReleasePackage
"C:\Program Files (x86)\7-Zip\7z.exe" a "Releases\ebcdic-compare-%date:~-4,4%%date:~-7,2%%date:~-10,2%.zip" "%~dp0Releases\Staging\*"
exit /b %errorlevel%