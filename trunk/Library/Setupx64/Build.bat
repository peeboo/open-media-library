@ECHO OFF

SET BUILD_TYPE=Release
if "%1" == "Debug" set BUILD_TYPE=Debug

REM Determine whether we are on an 32 or 64 bit machine
if "%PROCESSOR_ARCHITECTURE%"=="x86" if "%PROCESSOR_ARCHITEW6432%"=="" goto x86

set ProgramFilesPath=%ProgramFiles(x86)%

goto startInstall

:x86

set ProgramFilesPath=%ProgramFiles%

:startInstall
SET OUTPUTNAMECLIENT=..\bin\%BUILD_TYPE%\OMLSetupClientx64.msi
SET OUTPUTNAMESERVER=..\bin\%BUILD_TYPE%\OMLSetupServerx64.msi
pushd "%~dp0"

REM svn info > info.tmp
REM for /f "tokens=1,2" %%i in (info.tmp) do if "%%i" == "Revision:" set REVISION_NUMBER=%%j
REM del info.tmp
SET WIX_BUILD_LOCATION=%ProgramFilesPath%\Windows Installer XML v3\bin
SET APP_INTERMEDIATE_PATH=..\obj\%BUILD_TYPE%

REM Cleanup leftover intermediate files

if exist "%APP_INTERMEDIATE_PATH%\*.wixobj" del /f /q "%APP_INTERMEDIATE_PATH%\*.wixobj"
if exist "%OUTPUTNAMECLIENT%" del /f /q "%OUTPUTNAMECLIENT%"
if exist "%OUTPUTNAMESERVER%" del /f /q "%OUTPUTNAMESERVER%"

REM Build the MSI for the setup package

"%WIX_BUILD_LOCATION%\candle.exe" -ext WiXNetFxExtension -ext WiXUtilExtension setupclient.wxs setup.wxs -dBuildType=%BUILD_TYPE% -out %APP_INTERMEDIATE_PATH%\
"%WIX_BUILD_LOCATION%\light.exe" -ext WiXNetFxExtension -ext WixUIExtension -ext WiXUtilExtension "%APP_INTERMEDIATE_PATH%\setupclient.wixobj" "%APP_INTERMEDIATE_PATH%\setup.wixobj" -cultures:en-US -loc setup-en-us.wxl -out "%OUTPUTNAMECLIENT%"

"%WIX_BUILD_LOCATION%\candle.exe" -ext WiXNetFxExtension -ext WiXUtilExtension setupserver.wxs setup.wxs -dBuildType=%BUILD_TYPE% -out %APP_INTERMEDIATE_PATH%\
"%WIX_BUILD_LOCATION%\light.exe" -ext WiXNetFxExtension -ext WixUIExtension -ext WiXUtilExtension "%APP_INTERMEDIATE_PATH%\setupserver.wixobj" "%APP_INTERMEDIATE_PATH%\setup.wixobj" -cultures:en-US -loc setup-en-us.wxl -out "%OUTPUTNAMESERVER%"

echo Building Bootstrapper file
REM Commented out for now
REM MSBuild omlsetupx64.proj

echo Copying "%OUTPUTNAMECLIENT%" to the Builds folder
copy "%OUTPUTNAMECLIENT%" ..\..\Builds\X64\

echo Copying "%OUTPUTNAMESERVER%" to the Builds folder
copy "%OUTPUTNAMESERVER%" ..\..\Builds\X64\

popd
