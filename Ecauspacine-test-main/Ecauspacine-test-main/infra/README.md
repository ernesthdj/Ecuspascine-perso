# Infra — MySQL & phpMyAdmin (Docker)

MySQL 8 + phpMyAdmin, non exposés sur Internet (bind `127.0.0.1` sur le VPS).  
Accès **uniquement via tunnels SSH**.

## Fichiers

- `infra/docker-compose.yml`
- `infra/.env` (non committé — mots de passe)
- `db_init/` (scripts SQL d'init : création users/DB/permissions)

## Variables (.env — exemple)

```
PROJECT_NETWORK=ecauspacine_net
MYSQL_VERSION=8.0
MYSQL_PORT=3306
PHPMYADMIN_PORT=8080

MYSQL_ROOT_PASSWORD=***
MYSQL_DB=ecauspacine_dev

MYSQL_ADMIN_USER=db_admin
MYSQL_ADMIN_PASSWORD=***

MYSQL_TEAM_USER=dev_team
MYSQL_TEAM_PASSWORD=***

DB_DEV=ecauspacine_dev
DB_PROD=ecauspacine_prod
```

## Compose (rappel des points clés)

- MySQL écoute `127.0.0.1:3306` sur le VPS
- phpMyAdmin écoute `127.0.0.1:8080` sur le VPS
- Volumes persistants montés sous `/srv/school/ecauspacine/volumes/...`

## Sécurité

- **Aucune** ouverture de port publique
- Comptes MySQL :
  - `db_admin` : DDL sur DEV/PROD
  - `dev_team` : RW sur DEV, RO sur PROD
- `sshd_config` (extrait) :
  ```
  Match Group tunnelers
      AllowTcpForwarding yes
      PermitOpen 127.0.0.1:3306 127.0.0.1:8080 127.0.0.1:5000
      X11Forwarding no
      PermitTTY no
  ```
