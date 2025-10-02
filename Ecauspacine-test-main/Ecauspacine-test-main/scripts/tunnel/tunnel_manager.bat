@echo off
setlocal ENABLEDELAYEDEXPANSION
set "ROOT=%~dp0"
set "SCRIPTS=%ROOT%scripts"
set "PS=powershell -NoProfile -ExecutionPolicy Bypass"

:status
rem Determine UP/DOWN by pidfile + running process
%PS% -Command ^
  "$root='%ROOT%'; $pidfile=Join-Path $root 'tunnel.pid';" ^
  "if (Test-Path -LiteralPath $pidfile) {" ^
  "  $pid=[int](Get-Content -LiteralPath $pidfile | Select-Object -First 1);" ^
  "  if ($pid -and (Get-Process -Id $pid -ErrorAction SilentlyContinue)) { exit 0 }" ^
  "} ; exit 1"
if %errorlevel%==0 (set "STATE=UP") else (set "STATE=DOWN")

cls
echo =================================
echo   Ecauspacine Tunnel Manager
echo =================================
echo(
echo   Tunnel state : %STATE%
echo(
echo [S] Start   [K] Stop   [Q] Quit
set /p choice=Choice ^> 

if /I "%choice%"=="S" goto start
if /I "%choice%"=="K" goto stop
if /I "%choice%"=="Q" exit /b 0
goto status

:start
%PS% -File "%SCRIPTS%\start_tunnel.ps1" "%ROOT%"
echo.
pause
goto status

:stop
%PS% -File "%SCRIPTS%\stop_tunnel.ps1" "%ROOT%"
echo.
pause
goto status
