@echo off

start "" ".\ServerSoap\ServeurSOAP\bin\Debug\ServeurSOAP.exe"
timeout /t 2 /nobreak > nul
start "" ".\ProxySoap\ProxySoap\bin\Debug\ProxySoap.exe"
timeout /t 2 /nobreak > nul
activemq.bat start
pause