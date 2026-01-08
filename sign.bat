@echo off

set argC=0
for %%x in (%*) do Set /A argC+=1

set /A tries=3

rem the build task will fail if it sees any stderr printing, so 2> it.
signtool verify /pa /tw %1 1> nul 2> nul
if ERRORLEVEL 1 goto :try
echo %1 already signed.

if %argC% EQU 1 goto :eof
signtool verify /pa /tw %2 1> nul 2> nul
if ERRORLEVEL 1 goto :try
echo %2 already signed.

goto :eof

:retry
if %tries%==0 goto :barf
ping -n 2 127.0.0.1>nul

:try
set /A tries=%tries%-1
rem signtool sign /n "Collective Software" /i GlobalSign /t "http://timestamp.globalsign.com/scripts/timstamp.dll" /v %1 %2 1>>sign.out 2>>&1
rem signtool sign /n "Collective Software" /a /i GlobalSign /tr "http://timestamp.globalsign.com/tsa/r6advanced1" /td SHA256 /v %1 %2 1>>sign.out 2>>&1
signtool sign /n "Collective Software" /a /i GlobalSign /tr "http://timestamp.globalsign.com/tsa/r45standard" /td SHA256 /v %1 %2 1>>sign.out 2>>&1
if ERRORLEVEL 1 goto :retry
del sign.out >nul
echo Signed %1 %2 OK.
goto :eof

:barf
type sign.out
del sign.out >nul
exit /b 1