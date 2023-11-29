@echo off

start "" ".\ServerSoap\ServeurSOAP\bin\Debug\ServeurSOAP.exe"
timeout /t 2 /nobreak > nul
start "" ".\ProxySoap\ProxySoap\bin\Debug\ProxySoap.exe"
timeout /t 2 /nobreak > nul
start "" ".\Client_Test\ConsoleApp1\bin\Debug\ConsoleApp1.exe"

pause