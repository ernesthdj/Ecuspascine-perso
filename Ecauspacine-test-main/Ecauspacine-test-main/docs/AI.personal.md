
# AI-ASSISTANCE (Déclaration d’usage de l’IA — personnelle)

> Transparence : ce dépôt a été préparé et structuré **par moi (propriétaire du repo)** en m’aidant ponctuellement d’une IA (ChatGPT) pour gagner du temps sur la rédaction et l’outillage. Tout a été **relu, ajusté et validé manuellement** avant d’être commité.

## Rôle de l’IA (assistance, pas automatisation)

L’IA m’a aidé à :
- **Structurer le dépôt** (arborescence, dossiers `docs/`, `infra/`, `scripts/`, etc.).  
- **Rédiger / améliorer la documentation** : `README.md`, `CONTRIBUTING.md`, `SECURITY.md`, guides dans `docs/` (onboarding, accès DB via tunnel, Git/VS).  
- **Esquisser et itérer** les scripts **Windows (PowerShell/BAT)** pour le **tunnel SSH** (ouverture/fermeture, logs, ports MySQL/phpMyAdmin/API).  
- **Clarifier les workflows Git** (branches, PR, protection `main`).  
- Proposer des **bonnes pratiques** (sécurité `.env`, secrets hors Git, etc.).

## Ce que j’ai fait personnellement

- **Conception et décisions** d’architecture (choix VPS, MySQL dockerisé, accès via tunnel, etc.).  
- **Mise en place serveur** : comptes `tunnelers`, `sshd_config` (`PermitOpen`, `AllowTcpForwarding`), clés SSH, volumes Docker, init DB, droits MySQL.  
- **Tests et validation** des scripts sur **Windows** (PowerShell 5.1), ajustements des ports locaux et distants.  
- **Relecture / adaptation** de tous les textes et commandes pour notre contexte de cours.  
- **Publication** dans le repo, configuration des protections de branche et **release** des scripts.

> En bref : l’IA a été un **co-pilote** pour gagner du temps, **pas une source d’autorité**. Les choix finaux, vérifications et responsabilités sont les miens.

## Limites & responsabilité

- Le contenu généré par IA peut contenir des approximations ; **toute erreur m’appartient** et je corrige dès qu’un problème est signalé.  
- Les **secrets** (mots de passe, clés privées, `.env`) **ne sont jamais collés** dans l’IA ni committés. Les exemples utilisent des **placeholders**.  
- Les scripts/fichiers fournis sont **révisés** pour éviter la fuite d’information sensible (logs limités, chemins neutres).

## Données & confidentialité

- **Pas d’upload** de base de données ni de secrets dans l’IA.  
- Les captures et logs partagés sont **nettoyés** ou ne contiennent que des IP publiques non sensibles.  
- Les variables sensibles doivent rester en **local** (`infra/.env`, `appsettings.*.json` non versionné) et ne **doivent jamais** figurer sur GitHub.

## Contribution avec IA (si vous en utilisez)

Si vous proposez une PR rédigée/aidée par IA :
- **Indiquez-le** brièvement dans la description de la PR (transparence).  
- **Testez** vos scripts/commandes sur votre machine.  
- Ne collez **jamais** de secrets, tokens, clés privées ou dumps de DB.

## Contact

- Maintainer : **@TheoDscps** (GitHub) — ping me en issue/PR si besoin.
- Sujets urgents : ouvrez une **Issue** avec le label `help-wanted`.

---

_Version de cette note : 2025‑09‑07. Je mettrai à jour ce fichier si la politique d’usage de l’IA change._
