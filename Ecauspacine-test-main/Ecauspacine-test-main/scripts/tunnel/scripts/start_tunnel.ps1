param([string]$BasePath)

# --- Resolve base path (folder that contains tunnel_manager.bat) ---
try {
  if ($BasePath) {
    $BasePath = $BasePath.Trim('"')
    $BasePath = [System.IO.Path]::GetFullPath($BasePath)
  } else {
    $scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
    $BasePath   = [System.IO.Path]::GetFullPath( (Join-Path $scriptRoot '..') )
  }
} catch { }

# --- Logs beside the .bat ---
$logsDir = Join-Path $BasePath 'logs'
New-Item -ItemType Directory -Force -Path $logsDir | Out-Null
$ts     = Get-Date -Format 'yyyyMMdd_HHmmss'
$logOut = Join-Path $logsDir "ssh_out_$ts.log"
$logErr = Join-Path $logsDir "ssh_err_$ts.log"
$pidFile = Join-Path $BasePath 'tunnel.pid'

Write-Host "[INFO] PowerShell version: $($PSVersionTable.PSVersion.ToString())"

# --- Optional config file (key=value) ---
$configPath = Join-Path $BasePath 'config.env'
if (Test-Path $configPath) {
  foreach ($line in Get-Content -LiteralPath $configPath) {
    if ($line -match '^\s*#' -or $line -match '^\s*$') { continue }
    $kv = $line -split '=',2
    if ($kv.Count -eq 2) {
      $k = $kv[0].Trim()
      $v = $kv[1].Trim()
      if ($k) { Set-Variable -Name $k -Value $v -Scope Script }
    }
  }
}

# --- Identity prompts if missing ---
if (-not $VPS_USER) { $VPS_USER = Read-Host "Enter VPS_USER (e.g. thordev)" }
if (-not $VPS_IP)   { $VPS_IP   = Read-Host "Enter VPS_IP (e.g. 51.38.39.215)" }
Write-Host "[INFO] Using $VPS_USER@$VPS_IP"

# --- Defaults for ports ---
if (-not $MYSQL_LOCAL_PORT) { $MYSQL_LOCAL_PORT = 3307 }
if (-not $PMA_LOCAL_PORT)   { $PMA_LOCAL_PORT   = 8081 }
if (-not $API_LOCAL_PORT)   { $API_LOCAL_PORT   = 5001 }
if (-not $API_REMOTE_PORT)  { $API_REMOTE_PORT  = 5000 }

$localBind  = '127.0.0.1'
$remoteBind = '127.0.0.1'

# --- Build -L specs (avoid string parsing quirks in PS 5.1) ---
$L_mysql = ('{0}:{1}:{2}:{3}' -f $localBind, $MYSQL_LOCAL_PORT, $remoteBind, 3306)
$L_pma   = ('{0}:{1}:{2}:{3}' -f $localBind, $PMA_LOCAL_PORT,   $remoteBind, 8080)
$L_api   = ('{0}:{1}:{2}:{3}' -f $localBind, $API_LOCAL_PORT,   $remoteBind, $API_REMOTE_PORT)

$sshArgs = @(
  '-N','-4',
  '-L', $L_mysql,
  '-L', $L_pma,
  '-L', $L_api,
  "$VPS_USER@$VPS_IP"
)

# --- Launch ssh minimized with logs ---
$proc = Start-Process -FilePath 'ssh.exe' -ArgumentList $sshArgs `
        -WindowStyle Hidden -PassThru `
        -RedirectStandardOutput $logOut -RedirectStandardError $logErr

Start-Sleep -Milliseconds 400

# If it died instantly, fail with stderr tail
if ($proc.HasExited) {
  $exit = $proc.ExitCode
  $tail = ''
  if (Test-Path $logErr) {
    try { $tail = (Get-Content -LiteralPath $logErr -Tail 80) -join "`n" } catch {}
  }
  Write-Error "[ERROR] ssh exited immediately (code $exit).`n---- ssh stderr (tail) ----`n$tail"
  exit 1
}

# --- Health check: our three local ports are listening for THIS PID ---
function Test-PortByProcId([int]$port,[int]$procId){
  try {
    $conns = Get-NetTCPConnection -LocalPort $port -State Listen -ErrorAction Stop
    return [bool]($conns | Where-Object {$_.OwningProcess -eq $procId})
  } catch { return $false }
}

$ok = $false
for($i=0;$i -lt 50;$i++){  # ~5s max
  if ($proc.HasExited) {
    $exit = $proc.ExitCode
    $tail = ''
    if (Test-Path $logErr) { try { $tail = (Get-Content -LiteralPath $logErr -Tail 80) -join "`n" } catch {} }
    Write-Error "[ERROR] ssh exited (code $exit).`n---- ssh stderr (tail) ----`n$tail"
    exit 1
  }
  if( (Test-PortByProcId $MYSQL_LOCAL_PORT $proc.Id) -and
      (Test-PortByProcId $PMA_LOCAL_PORT   $proc.Id) -and
      (Test-PortByProcId $API_LOCAL_PORT   $proc.Id) ) { $ok=$true; break }
  Start-Sleep -Milliseconds 100
}

if (-not $ok) {
  try { Stop-Process -Id $proc.Id -Force -ErrorAction SilentlyContinue } catch {}
  $tail = ''
  if (Test-Path $logErr) { try { $tail = (Get-Content -LiteralPath $logErr -Tail 80) -join "`n" } catch {} }
  Write-Error "[ERROR] Tunnel did not come UP (ports not listening).`n---- ssh stderr (tail) ----`n$tail"
  exit 2
}

# success
Set-Content -LiteralPath $pidFile -Value $proc.Id
Write-Host "[OK] Tunnel started (PID $($proc.Id))."
exit 0
