param([string]$BasePath)

try {
  if ($BasePath) {
    $BasePath = $BasePath.Trim('"')
    $BasePath = [System.IO.Path]::GetFullPath($BasePath)
  } else {
    $scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
    $BasePath   = [System.IO.Path]::GetFullPath( (Join-Path $scriptRoot '..') )
  }
} catch { }

$pidFile = Join-Path $BasePath 'tunnel.pid'
Write-Host "[INFO] Stopping tunnel..."

if (Test-Path -LiteralPath $pidFile) {
  $raw = (Get-Content -LiteralPath $pidFile | Select-Object -First 1)
  $sshPid = $raw -as [int]
  if ($sshPid -and (Get-Process -Id $sshPid -ErrorAction SilentlyContinue)) {
    Stop-Process -Id $sshPid -Force
    Write-Host "[OK] Tunnel stopped (PID $sshPid)."
  } else {
    Write-Host "[INFO] No running ssh with PID from pidfile. Trying to stop any orphan ssh..."
    Get-Process ssh -ErrorAction SilentlyContinue | Stop-Process -Force -ErrorAction SilentlyContinue
  }
  Remove-Item -LiteralPath $pidFile -ErrorAction SilentlyContinue
} else {
  Write-Host "[INFO] No pidfile; nothing to stop."
}
