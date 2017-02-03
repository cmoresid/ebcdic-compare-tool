REM Create a 'GeneratedReports' folder if it does not exist
if not exist "%~dp0GeneratedReports" mkdir "%~dp0GeneratedReports"
 
REM Remove any previous test execution files to prevent issues overwriting
IF EXIST "%~dp0TestResults\EbcdicCompare.trx" del "%~dp0TestResults\EbcdicCompare.trx%"
 
REM Remove any previously created test output directories
CD %~dp0
FOR /D /R %%X IN (%USERNAME%*) DO RD /S /Q "%%X"
 
REM Run the tests against the targeted output
call :RunOpenCoverUnitTestMetrics
 
REM Generate the report output based on the test results
if %errorlevel% equ 0 (
 call :RunReportGeneratorOutput
)
 
REM Launch the report
if %errorlevel% equ 0 (
 call :RunLaunchReport
)
exit /b %errorlevel%
 
:RunOpenCoverUnitTestMetrics
"%~dp0packages\OpenCover.4.6.519\tools\OpenCover.Console.exe" ^
-register:user ^
-target:"C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\mstest.exe" ^
-targetargs:"/testsettings:\"%~dp0CI_Config.testsettings\" /noresults /noisolation /testcontainer:\"%~dp0CodeMovement.EbcdicCompare.UnitTests\bin\Debug\CodeMovement.EbcdicCompare.UnitTests.dll\" /resultsfile:\"%~dp0TestResults\EbcdicCompare.trx\"" ^
-filter:"+[CodeMovement.EbcdicCompare*]* -[CodeMovement.EbcdicCompare.Presentation]*View" ^
-excludebyattribute:*.ExcludeFromCodeCoverage* ^
-hideskipped:All ^
-output:"%~dp0\GeneratedReports\CodeMovement.EbcdicCompare.xml"
exit /b %errorlevel%
 
:RunReportGeneratorOutput
"%~dp0packages\ReportGenerator.2.5.2\tools\ReportGenerator.exe" ^
-reports:"%~dp0\GeneratedReports\CodeMovement.EbcdicCompare.xml" ^
-targetdir:"%~dp0\GeneratedReports\ReportGenerator Output"
exit /b %errorlevel%
 
:RunLaunchReport
start "report" "%~dp0\GeneratedReports\ReportGenerator Output\index.htm"
exit /b %errorlevel%