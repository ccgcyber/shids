@ECHO OFF
set SERVICE=S HIDS Client Service
net stop "%SERVICE%"
sc delete "%SERVICE%"
set DOTNETFX2=C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\
set PATH=%PATH%;%DOTNETFX2%
echo Installing WindowsService...
echo ---------------------------------------------------
InstallUtil /i shids_client.exe
echo ---------------------------------------------------
echo Done
net start "%SERVICE%"
@ECHO OFF
@pause