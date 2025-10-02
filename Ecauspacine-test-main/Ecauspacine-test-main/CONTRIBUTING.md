# Contribuer à **Ecauspacine**

Merci de contribuer au projet 🎉  
Ce guide explique **comment travailler proprement**, ouvrir des **PR** et garder une base **stable**.

---

##  TL;DR (rapide)
1. **Crée une branche** depuis `develop` : `feature/<sujet>` ou `fix/<bug>`.
2. **Code** (API .NET / WinForms). Garde la logique métier hors de l’UI.
3. **Teste en local** (tunnel SSH → MySQL / API), vois `docs/db-access-dev.md` et `docs/visual-studio-workflow.md`.
4. **Commit** avec un message clair _Conventional Commits_ (ex: `feat: écran de login`).
5. **Ouvre une PR vers `develop`**, remplis le descriptif et demande une relecture.
6. Quand la PR est OK → **merge**. `main` reste **stable / livrable**.

---

##  Stratégie de branches

| Branche      | Rôle                                                                 |
|--------------|----------------------------------------------------------------------|
| `main`       | **Stable**. Ce qui peut être montré/ livré au prof.                 |
| `develop`    | Intégration des features (pré-prod interne).                         |
| `feature/*`  | Nouveau développement (ex: `feature/login-form`).                    |
| `fix/*`      | Correction de bug (ex: `fix/crash-connexion`).                       |
| `docs/*`     | Docs uniquement.                                                     |
| `chore/*`    | Tâches de maintenance (outillage, scripts, CI…).                     |

**Création depuis VS/CLI** :
```bash
# à partir de develop
git switch develop
git pull
git switch -c feature/mon-sujet
```

---

##  Commits (Conventional Commits)

Types courants : `feat`, `fix`, `docs`, `chore`, `refactor`, `test`

Exemples :
```text
feat: écran de login (winforms) + service auth
fix: validation DTO RegisterUser
docs: ajouter guide tunnel SSH
chore: Script release GitHub
refactor: extraire HttpClientFactory
test: ajouter tests ApiUserService
```

Astuce : **commits petits et fréquents** (facile à relire/revert).

---

##  Pull Requests (PR)

**Avant d’ouvrir la PR** :
- [ ] Rebase sur `develop` (`git pull --rebase`).
- [ ] Build OK en local.
- [ ] Tests manuels (et unitaires si présents).

**Dans la PR** :
- Contexte : **quoi / pourquoi**.
- Changements clés + **comment tester**.
- Lier une issue si besoin : `Fixes #123`.
- Demander **1 relecture** (idéalement).

**Règles** :
- Cible = `develop`.
- Pas de fichiers secrets (.env, appsettings secrets, …).
- CI (à venir) doit passer.

---

##  Règles de code

### API (.NET, C# 12)
- REST clair, statuts HTTP corrects, DTOs + **validation**.
- Séparer **Domain / Application / Infrastructure** (au minimum Services vs Controllers).
- `ILogger<>`, gestion d’erreurs (middleware).  
- Connexions : `appsettings.Development.json` ou **User Secrets**.

**Exemple `appsettings.Development.json` (local via tunnel)** :
```json
{
  "ConnectionStrings": {
    "Default": "Server=127.0.0.1;Port=3307;Database=ecauspacine_dev;User ID=dev_team;Password=<MDP>;TreatTinyAsBoolean=false"
  },
  "Api": {
    "BaseUrl": "http://localhost:5001"
  }
}
```

### WinForms (Front)
- UI = présentation seulement. **Pas de logique métier** dans les Form.  
- Utiliser un pattern de séparation (MVVM-like ou **Presenter/Service** simple).
- Un **HttpClient** centralisé (`HttpClientFactory` ou wrapper).
- Afficher clairement **les erreurs** (timeouts, 4xx/5xx).

---

##  Tests
- Ajoutez des **tests unitaires** pour la logique critique (quand possible).
- Pour l’instant : documenter **tests manuels** dans la PR (scénarios / captures).

---

##  Infra (rappels)
- MySQL (DEV) accessible via **tunnel** → `127.0.0.1:3307`.
- phpMyAdmin via **tunnel** → `http://localhost:8081`.
- API (proxy local) via **tunnel** → `http://localhost:5001` (redirigé vers VPS:5000).
- Ne changez pas les ports/infra **sans accord** de l’admin infra.

---
> En cas de blocage, ouvrez une **Issue** (template *Help request*) et joignez logs/erreurs.
