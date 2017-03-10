@ECHO OFF
set SERVICE=S HIDS Client Tracker Service
net stop "%SERVICE%"
sc delete "%SERVICE%"
REM The following directory is for .NET 2.0
set DOTNETFX2=%SystemRoot%\Microsoft.NET\Framework\v2.0.50727
set PATH=%PATH%;%DOTNETFX2%

echo Installing WindowsService...
echo ---------------------------------------------------
InstallUtil /i TrackerService.exe
echo ---------------------------------------------------
echo Done.
net start "%SERVICE%"
@pause