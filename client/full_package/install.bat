@echo off
echo #############################################################
echo	 ######                ##     ## #### ########   ######  
echo	##    ##               ##     ##  ##  ##     ## ##    ## 
echo	##                     ##     ##  ##  ##     ## ##       
echo	 ######     #######    #########  ##  ##     ##  ######  
echo	      ##               ##     ##  ##  ##     ##       ## 
echo	##    ##               ##     ##  ##  ##     ## ##    ## 
echo	 ######                ##     ## #### ########   ######  
echo #############################################################
echo           Windows Agent Installation - V 0.1 Alpha
echo #############################################################
@pause
SET FOLDER_PATH=%SystemDrive%\shids_win_agent
set CURRENT_PATH=%~dp0
set PATH=%PATH%;%systemroot%\system32

copy /Y service_building.bat.hids service_building.bat

REM copy /Y shids_client.exe.config.hids shids_client.exe.config
copy /Y shids_client.exe.hids shids_client.exe
copy /Y TrackerService.exe.hids TrackerService.exe

copy /Y buidler.exe.hids buidler.exe
copy /Y dbBuilder.exe.hids dbBuilder.exe

copy /Y DetectDotNet.exe.hids DetectDotNet.exe
copy /Y dotnet20.exe.hids dotnet20.exe
copy /Y WindowsInstaller.exe.hids WindowsInstaller.exe

del /F /Q *.hids
mkdir %FOLDER_PATH%
copy /Y service_building.bat %FOLDER_PATH%\service_building.bat
REM copy /Y shids_client.exe.config %FOLDER_PATH%\shids_client.exe.config
copy /Y shids_client.exe %FOLDER_PATH%\shids_client.exe
copy /Y TrackerService.exe %FOLDER_PATH%\TrackerService.exe
copy /Y buidler.exe %FOLDER_PATH%\buidler.exe
copy /Y dbBuilder.exe %systemroot%\system32\dbBuilder.exe

del /F /Q service_building.bat

DetectDotNet.exe

REG ADD "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Run" /v tmp_shids_s  /d "%FOLDER_PATH%\service_building.bat %FOLDER_PATH%"
echo "Restarting required !!!"
@pause
del /F /Q *.exe
SHUTDOWN -r -t 0


