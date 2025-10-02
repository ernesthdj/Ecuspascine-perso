# Ecauspacine

Application scolaire composée d’une **API .NET (REST)**, d’un **front WPF** et d’une **base MySQL** hébergée sur un **VPS**.  
Par sécurité, **la base n’est pas exposée sur Internet** : on y accède via un **tunnel SSH**.

---

##  Ce que vous allez faire (pas à pas)

1. **[Première contribution](CONTRIBUTORS.md)** 
2. **Créer votre clé SSH** et l’envoyer à l’admin.  
3. **Télécharger et lancer** les **scripts de tunnel** (Windows).  
4. **Tester** l’accès à MySQL / phpMyAdmin / API.  
5. **Cloner le dépôt** et travailler sur **votre branche**.

> Tout est détaillé, étape par étape, dans **[docs/onboarding.md](docs/onboarding.md)**.  
> Le workflow Git/Visual Studio simple pour l’équipe est dans **[docs/git-team-playbook.md](docs/git-team-playbook.md)**.

---

##  Démarrage ultra-rapide (récap)

- Demandez l’accès **SSH** (envoyez votre clé publique) → voir *Onboarding*.  
- Téléchargez les scripts de tunnel (menu **Releases** du dépôt) :  
  **Ecauspacine_tunnel_scripts_v1.zip** → dézip → **double‑cliquez `tunnel_manager.bat`**.  
- Vérifiez :
  - **MySQL** : `127.0.0.1:3307`
  - **phpMyAdmin** : `http://localhost:8081`
  - **API (DEV/proxy)** : `http://localhost:5001` *(si l’API est déployée sur le VPS)*
- Clonez le repo et **travaillez sur une branche** (`feature/...` ou `fix/...`).  
- Ouvrez une **Pull Request** pour intégrer vos changements dans `main` (revue obligatoire).

---

##  Plan du dépôt

```
/ (racine)
├─ README.md
├─ docs/
│  ├─ onboarding.md                 # tutos pas-à-pas (clé SSH, tunnel, tests, dépannage)
│  └─ git-team-playbook.md          # Git + Visual Studio pour l’équipe (hyper détaillé)
├─ scripts/
│  └─ tunnel/                       # scripts Windows pour ouvrir/fermer le tunnel SSH
├─ api/                             # projet .NET Web API
└─ wpf/                             # projet WPF (.NET)
```

---

##  Connexions (DEV)

- **MySQL** : `Server=127.0.0.1;Port=3307;Database=ecauspacine_dev;User ID=dev_team;Password=<MDP>;`
- **phpMyAdmin** : `http://localhost:8081`
- **API (local via tunnel)** : `http://localhost:5001`  → redirigé vers `127.0.0.1:5000` (VPS)

> En PROD (sur le VPS), l’API parlera à MySQL en interne (ex. `Server=db;Port=3306;Database=ecauspacine_prod;...`).

---

##  Sécurité (essentiel)

- Accès DB **uniquement via SSH** (groupe *tunnelers*, `PermitOpen` restreint).  
- **Aucune donnée sensible dans Git**. Utilisez :
  - `infra/.env` (copie depuis `.env.example`)
  - `api/appsettings.Development.json` (non commité)

---

##  Workflow Git (résumé)

- **Branche** par feature : `feature/<desc>` ; par bugfix : `fix/<desc>`  
- **PR requise** pour `main` (revue par le propriétaire).  
- **Visual Studio** : *Git* → *Create new branch* → *Commit* → *Push* → *Create Pull Request*.

 Guide détaillé (pas-à-pas) : **[docs/git-team-playbook.md](docs/git-team-playbook.md)**

---

## Support & Dépannage

- Tunnel, erreurs SSH, tests de port : **[docs/onboarding.md](docs/onboarding.md)** (FAQ en bas).  
- En cas de souci tunnel, transmettez **la commande** utilisée, **le message d’erreur** et **votre OS**.

---

##  Notes

- Les scripts de tunnel sont fournis pour **Windows** (PowerShell + .bat).  
- Mac/Linux : possible de faire équivalent en CLI `ssh -N -4 -L ...` (voir *Onboarding*).

#### Transparence
 Par soucis de transparence sur l'utilisation de l'IA cette section sera mise à jour suivant les utilisations personnelles ou groupées de l'Intelligence Artificielle.
 
  - **[docs/AI.personal.md](docs/AI.personal.md)**
