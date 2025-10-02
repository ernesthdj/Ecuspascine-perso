# Onboarding — Accès au VPS & clés SSH

## 1) Générer votre clé SSH

### Windows 10/11 (PowerShell)
```powershell
ssh-keygen -t ed25519 -C "prenom.nom@ecauspacine"
# Validez le chemin par défaut (~/.ssh/id_ed25519) (passphrase pour plus de sécurité, sinon laisser la passphrase vide)
Get-Content $env:USERPROFILE\.ssh\id_ed25519.pub | Set-Clipboard
```
Collez le contenu dans votre message à l’admin.

### macOS / Linux
```bash
ssh-keygen -t ed25519 -C "prenom.nom@ecauspacine"
pbcopy < ~/.ssh/id_ed25519.pub        # macOS
# ou
xclip -sel clip < ~/.ssh/id_ed25519.pub  # Linux (si xclip installé)
```

> **Ne partagez jamais la clé privée** (`id_ed25519`). Seule la clé **.pub** est à envoyer.

## 2) Demander l’accès

Envoyez à l’admin :
- votre **login souhaité** (ex. `alice`),
- votre **clé publique** (le contenu de `id_ed25519.pub`).

Vous recevrez un compte de type tunnel-only : `alice@<VPS_IP>`.

Pour la première connexion entrez dans un terminal powershell
```bash
ssh -N -4 -L 127.0.0.1:8081:127.0.0.1:8080 login@51.38.39.215
```

On vous demande si vous autorisez la connexion : entrez ```yes``` en toute lettre. Sans refermer le terminal allez sur **phpMyAdmin** : http://localhost:8081 vous devriez voir l'interface de connexion phpMyAdmin.
Vous pouvez tout refermer.


## 3) Installer les scripts du tunnel

### Récupérer les scripts
- Allez sur l’onglet **Releases** du dépôt GitHub.  
- Téléchargez **Ecauspacine_tunnel_scripts_v1.zip**.  
- **Dézippez** (ex. dans `C:\Users\<vous>\Downloads\Ecauspacine_tunnel_scripts_v1\`).  

Vous verrez notamment :
```
Ecauspacine_tunnel_scripts_v1/
├─ tunnel_manager.bat    <- lanceur
└─ scripts/
   ├─ start_tunnel.ps1
   └─ stop_tunnel.ps1
```

###  Démarrer
1. **Double‑cliquez** `tunnel_manager.bat`.  
2. Choisissez **S** (*Start*).  
3. Entrez votre **VPS_USER** (ex. `thordev`) et **VPS_IP** (ex. `51.38.39.215`).  
4. Laissez la fenêtre **ouverte** (minimisée si vous voulez).

Le script ouvre **3 redirections locales** :
- `127.0.0.1:3307` → MySQL sur le VPS (`127.0.0.1:3306`)
- `127.0.0.1:8081` → phpMyAdmin sur le VPS (`127.0.0.1:8080`)
- `127.0.0.1:5001` → API (DEV) sur le VPS (`127.0.0.1:5000`) — *si autorisée côté serveur*

###  Arrêter
- Relancez `tunnel_manager.bat` → **K** (*Stop*), ou  
- Fermez la fenêtre du tunnel si vous avez ouvert manuellement via `ssh`.

###  Où sont les logs ?
- Le manager affiche les erreurs **à l’écran**.  
- Des logs simples peuvent aussi être écrits dans `scripts\logs\` (selon version).  
- Transmettez le **message exact** à l’admin en cas de souci.

---

## 4) Vérifier que ça marche

- **phpMyAdmin** : ouvrez `http://localhost:8081` dans un navigateur.  
  - Identifiants : `dev_team` (MDP fourni par l’admin).  
- **MySQL** depuis un client (DBeaver, Workbench) :  
  - Host : `127.0.0.1` — Port : `3307` — DB : `ecauspacine_dev`  
- **API (si dispo)** : `http://localhost:5001` (une route `/api/health` peut exister selon l’implémentation).

---

##  Problèmes fréquents (Dépannage)

### 1) *“administratively prohibited: open failed”*
- L’admin doit autoriser le port via `PermitOpen` côté serveur.  
- Vérifier que votre user est dans le groupe **tunnelers** et que les bons ports sont listés.

### 2) *“Permission denied (publickey)”*
- La clé publique n’est pas installée ou n’est pas celle utilisée par votre PC.
- Vérifiez la présence de `~/.ssh/id_ed25519` **et** `~/.ssh/id_ed25519.pub`.
- Sur Windows, lancez l’agent si besoin :
  ```powershell
  Start-Service ssh-agent
  ssh-add
  ```

### 3) *“Address already in use”*
- Un autre programme utilise déjà le port local (3307/8081/5001).  
- Solution : choisissez d’autres ports **locaux** (ex. 3308/8082/5002).

### 4) *127.0.0.1 ne marche pas mais localhost oui (Windows)*
- Lancez le tunnel avec l’option **`-4`** et un **bind 127.0.0.1** explicite.  
  → Nos scripts le font déjà.

### 5) Je ne vois pas d’erreur mais rien ne répond
- Ouvrez `http://localhost:8081` : si la page se charge → tunnel OK côté web.  
- Testez les ports en PowerShell :
  ```powershell
  Test-NetConnection 127.0.0.1 -Port 3307
  Test-NetConnection 127.0.0.1 -Port 8081
  Test-NetConnection 127.0.0.1 -Port 5001
  ```
- Envoyez le **message d’erreur** exact affiché dans le manager à l’admin.

---

##  Bonus — Comprendre en 20 secondes

- **SSH** = canal sécurisé entre votre PC et le VPS.  
- **Tunnel SSH** = redirection d’un port local vers une adresse/port côté serveur.  
- **Pourquoi ?** Exposer la DB sur Internet est risqué ; on passe par SSH, c’est **fermé et chiffré**.

---

##  Besoin d’aide ?
- Demandez sur le canal projet.  
- Envoyez **la commande**, **l’erreur** et **votre OS**. Plus c’est précis, plus c’est rapide à résoudre.

