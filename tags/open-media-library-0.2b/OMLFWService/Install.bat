SETLOCAL ON
SET lPath=Release
IF "%1" EQU "Debug" GOTO :Debug
GOTO install
SHIFT /1
:Debug
SET lPath=Debug
:install
pushd C:\Develop\Projects\CS\OMLWatcher2008\OMLFWService\bin\%lPath%
C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\installutil %1 C:\Develop\Projects\CS\OMLWatcher2008\OMLFWService\bin\%lPath%\omlfwservice.exe
popd
