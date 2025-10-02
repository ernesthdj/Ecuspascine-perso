# Git & Visual Studio — Playbook d’équipe (débutant friendly)

Ce guide explique **exactement** quoi cliquer/taper pour travailler à plusieurs **sans casser `main`**.

---

## 0) Pré-requis (une fois)
### Via Powershell
- **Git** installé : https://git-scm.com/download/win  
- Configurer votre identité (dans PowerShell) :
  ```powershell
  git config --global user.name  "Prénom Nom"
  git config --global user.email "prenom.nom@exemple.com"
  git config --global core.autocrlf true
  ```
- Accès au repo GitHub (**vous devez être ajouté** comme collaborateur).
### Via VS2022 (**recommandé**)
 - Lors de la connexion à votre compte github tout se fait automatiquement
---

## 1) Cloner le dépôt

### Option A — Visual Studio 2022
1. Ouvrez Visual Studio → **Git** → **Clone a repository**.  
2. Collez l’URL du repo, choisissez un dossier local, cliquez **Clone**.

### Option B — PowerShell
```powershell
cd D:\Repos
git clone https://github.com/<org_ou_user>/Ecauspacine.git
cd Ecauspacine
```

---

## 2) Créer votre branche de travail

> **Jamais** de commits directs sur `main`. On travaille sur **sa propre branche** puis on ouvre une **Pull Request**.

### Visual Studio
- **Git** → **New Branch…**  
  - Name : `feature/nom-court` ou `fix/bug-xxx`
  - Based on branch : `main`
  - **Create** (et **Checkout** coché)

### PowerShell
```powershell
git checkout main
git pull origin main
git switch -c feature/nom-court
```
### Avant de coder : mettre à jour votre copie (Visual Studio)

> Toujours synchroniser avant de commencer une session de travail.

- Fetch
  	Git → Fetch (récupère les nouveautés sans les appliquer).

- Mettre main à jour

  - Bascule sur main : barre de statut (en bas) → clique le nom de branche → main.

  - Git → Pull (ta branche main locale = dernière version distante).

- Remettre ta branche à jour par rapport à main
  	Reviens sur ta branche (ex. feature/eleves-crud) puis :

  - Option simple (merge) : Git → Manage Branches… → clic droit sur ta branche → Merge From… → choisis main → Merge.

  - Option plus propre (rebase) : Git → Manage Branches… → clic droit sur ta branche → Rebase Onto… → choisis main → Start Rebase → résous les conflits si besoin.

> Astuce : fais ce cycle Fetch → Pull main → Rebase/Merge vers ta branche à chaque reprise de travail pour éviter les grosses surprises au moment de la PR.
---

## 3) Commits réguliers

### Visual Studio
> Objectif : petits commits clairs, faciles à relire/rollback.

### A. Ouvrir l’onglet Git Changes
`View → Git Changes` (ou l’icône Git). Tu vois :

  - Changes : fichiers modifiés non “stagés”
  - Staged Changes : ce qui partira dans ce commit

### B. Relire et stager

- Clique un fichier → Visual Studio montre le diff.

- Stage fichier : clic droit sur le fichier → Stage.

- Stage par lignes : dans le diff, sélectionne des lignes → Stage Selected Lines (si visible).

- Répète jusqu’à ne regrouper que les changements cohérents pour ce commit.

> Règle d’or : 1 idée = 1 commit.
> Exemple : “ajout du service X” et “correction d’un bug UI” → 2 commits séparés.

### C. Écrire un bon message (Conventional Commits)
Format recommandé :

```php-template
<type>(<scope>): <résumé court>

<détails utiles : pourquoi, quoi, comment tester>

Fixes #<numéro d’issue>   (optionnel)
```

Types courants : `feat`, `fix`, `docs`, `chore`, `refactor`, `test`, `perf`.

#### Exemples concrets
```bash
feat(api): ajouter StudentsController (CRUD basique)

Expose GET/POST/PUT/DELETE sur /api/vaisseau.
Valide les DTO (nom requis), renvoie 201 à la création.
Ajoute /api/health pour les checks.

Fixes #12
```
```bash
fix(frontend): éviter NullReference si aucun vaisseau sélectionné

Désactive le bouton "Modifier" quand la sélection est vide.
Guard clause dans ShipsViewModel.UpdateCommand.
```

### D. Commit & Push

- Clique Commit Staged (ou Commit All si tu n’utilises pas la staging area).

- Puis Push (icône flèche ↑ ou Git → Push).

- Si c’est la première fois sur cette branche : Publish te sera proposé.

### E. Diviser en plusieurs commits (cas fréquent)

- Stage uniquement le lot A → Commit.

- Stage uniquement le lot B → Commit.

- Push (après 1 ou plusieurs commits).

### F. Corriger le dernier commit (avant Push)

- Coche Amend (ou “Amend Last Commit”) dans Git Changes → ajuste message/fichiers → Commit.

    > À utiliser uniquement si tu n’as pas push.

### G. Annuler un fichier modifié

- Git Changes → clic droit fichier → Discard Changes (revient à la dernière version commitée).

### H. Revenir en arrière après Push (revert)

- Git → Manage Branches… → Branch History → clic droit sur le commit → Revert (crée un commit inverse).

### PowerShell
```powershell
git status
git add <fichiers>
git commit -m "feat: titre court qui explique"
```

---

## 4) Pousser et ouvrir une Pull Request

### Visual Studio
1. **Push** (icône flèche ↑ ou *Git → Push*).  
2. **Create Pull Request** (bouton proposé par VS) → remplissez le formulaire.

### GitHub (web)
1. Allez sur le repo → onglet **Pull requests** → **New pull request**.  
2. Base: `main` ← Compare: *votre branche* → **Create pull request**.  
3. Décrivez ce que vous avez fait, **comment tester**, et liez l’issue si besoin.  
4. Demandez la review (par défaut, **le propriétaire** doit approuver).

### PowerShell
```powershell
git push -u origin feature/nom-court
# puis ouvrez la PR dans l'interface GitHub
```

---

## 5) Récupérer les changements des autres

Toujours **mettre à jour** votre branche **avant** de continuer :
```powershell
git fetch
git pull --rebase
```
Si Git vous dit de faire un merge, préférez **`--rebase`** (plus propre) sauf cas particulier.

---

## 6) Conflits (basique)

- Visual Studio met les fichiers en conflit dans **Git Changes**.  
- Ouvrez chaque fichier → choisissez **Accept Current / Accept Incoming / Both**.  
- Compilez/testez → **Commit** → **Push**.

---

## 7) Règles maison

- **Pas de secrets** dans Git (mots de passe, `.env`, `appsettings.*.json` sensibles).  
- **Commits petits et fréquents**.  
- **Nommez vos branches clairement**.  
- **PRs** : ajoutez *comment tester* + *screenshots* si UI.

---

## 8) Visual Studio — trucs utiles

- **Solution** séparée pour `api/` et `winforms/` (vous pouvez ouvrir la solution désirée).  
- **Gestionnaire de branches** : *Git → Manage Branches…*  
- **Synchroniser** : *Git → Fetch/Pull/Push*

---

## 9) Exemple complet (PowerShell)

```powershell
# 1) cloner
git clone https://github.com/<org_ou_user>/Ecauspacine.git
cd Ecauspacine

# 2) créer une branche
git switch -c feature/health-endpoint

# 3) coder… puis
git add api/Controllers/HealthController.cs
git commit -m "feat(api): add /api/health endpoint"

# 4) pousser et ouvrir PR
git push -u origin feature/health-endpoint
# → ouvrez la PR dans GitHub
```

---

## 10) Besoin d’aide ?
- Demandez au propriétaire du repo.  
- Joignez **la commande**, **le message d’erreur** et **ce que vous vouliez faire**.
