# Scripts — Tunnel SSH

Outils pour **ouvrir/fermer** les tunnels nécessaires à MySQL, phpMyAdmin et l’API.

## Fichiers

- `tunnel_manager.bat` — menu simple (Start/Stop/Quit)
- `scripts/start_tunnel.ps1` — ouvre les 3 tunnels
- `scripts/stop_tunnel.ps1` — stoppe le tunnel courant

## Utilisation
1. Double-cliquez `tunnel_manager.bat`
2. **S** pour *Start* ; saisissez `VPS_USER` et `VPS_IP` si demandé
3. **K** pour *Stop* (ferme le tunnel lancé par Start)

### Ports (par défaut)
- Local **3307** → VPS **3306** (MySQL)
- Local **8081** → VPS **8080** (phpMyAdmin → http://localhost:8081)
- Local **5001** → VPS **5000** (API → http://localhost:5001)

## Clés SSH — rappel

### Windows (PowerShell)
```powershell
ssh-keygen -t ed25519 -C "prenom.nom@ecauspacine"
Get-Content $env:USERPROFILE\.ssh\id_ed25519.pub | Set-Clipboard
```
Envoyez **la clé .pub** à l’admin qui créera votre compte *tunneler*.

## Dépannage rapide

- **404 / impossible d’ouvrir http://localhost:8081** : le tunnel n’est pas lancé ? Port local déjà pris ?
- **Auth SSH** : clé publique bien installée côté VPS ?
- **MySQL** : utilisez **127.0.0.1:3307** et l’utilisateur `dev_team` pour DEV.
