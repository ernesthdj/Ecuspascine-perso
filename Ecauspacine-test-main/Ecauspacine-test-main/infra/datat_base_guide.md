# Guide simple de la base de données

## 1) L’idée
On décrit **des types d’entités** (ex. “ShipModel”) et **des attributs réutilisables** (ex. “Name”, “MaxSpeed”).  
Chaque entité créée stocke ses **valeurs** dans une structure **EAV typée** (Entity–Attribute–Value) pour gérer *tous* les types (texte, nombre, booléen, lookup, etc.).

---

## 2) Les blocs de base (qui fait quoi ?)

- **`entity_type`** — *Quelles classes existent ?*  
  Ex. `ShipModel` (le champ **code** = nom de classe C#).

- **`attribute_def`** — *Quels attributs sont possibles, et de quel type ?*  
  Ex. `Name` (texte), `MaxSpeed` (double).  
  (**code** = nom de propriété C# ; **data_kind** = type logique : string, int, double, bool, json, lookup, ref_entity…)

- **`attribute_rule`** — *Quels attributs s’appliquent à un type d’entité, et comment ?*  
  Lie un `attribute_def` à un `entity_type` + options :  
  - **is_required** (obligatoire ?),  
  - **order_index** (ordre d’affichage / export),  
  - **is_private_set** (accesseur C# :  
    `false` → `public { get; set; }`,  
    `true`  → `public { get; private set; }`).

- **`entity`** — *Les instances réelles*  
  Ex. “Corvette MK1” est une ligne liée à `entity_type = ShipModel`.

- **`attribute_val`** — *Le pivot E↔A*  
  1 ligne par (entité, attribut). La **valeur** est stockée à côté, dans **une seule** table typée.

- **Tables de valeurs typées** — *Où va la valeur selon le type ?*
  - Entier → `attribute_val_num_integer.value_bigint`
  - Réel (float/double) → `attribute_val_num_float.value_double`
  - Booléen → `attribute_val_bool.value_bool`
  - Texte → `attribute_val_text.value_text`
  - JSON → `attribute_val_json.value_json`
  - Lookup (référentiel) → `attribute_val_lookup.lookup_item_id`
  - Référence d’entité → `attribute_val_ref_entity.ref_entity_id`

- **Référentiels : `lookup_group` / `lookup_item`** — *Les listes de valeurs*  
  Ex. groupe `relation_type` avec des items `Requires`, `UpgradesTo`, …

- **`entity_relation`** — *Relations entre entités (M:N)*  
  Stocke `from_entity` → `to_entity` avec un **type** (FK vers `lookup_item` du groupe `relation_type`).

---

## 3) Comment ça s’utilise (pas à pas)

### A) Définir la structure
1) Créer un **type d’entité** : `entity_type.code = ShipModel`.  
2) Créer des **attributs** (catalogue) :  
   - `Name` → `data_kind = string`  
   - `MaxSpeed` → `data_kind = double`
3) Ajouter des **règles** pour ce type :  
   - `ShipModel` ← `Name` (`is_required=true`, `order_index=10`, `is_private_set=false`)  
   - `ShipModel` ← `MaxSpeed` (`is_required=false`, `order_index=20`, `is_private_set=true`)

### B) Créer des données
1) Créer une **entité** “Corvette MK1” (ligne dans `entity`).  
2) Pour chaque attribut appliqué par `attribute_rule` :  
   - Créer (ou upserter) une ligne dans `attribute_val` (le pivot),  
   - Écrire **la valeur** dans **la table typée correspondante** (une seule !).  
     - `Name` (string) → `attribute_val_text.value_text = "Corvette MK1"`  
     - `MaxSpeed` (double) → `attribute_val_num_float.value_double = 320.5`

---

## 4) Types gérés (simple et clair)

| Besoin | Choix `data_kind` | Où va la valeur |
|---|---|---|
| Texte | `string`/`text` | `attribute_val_text.value_text` |
| Booléen | `bool` | `attribute_val_bool.value_bool` |
| Entier (sbyte/byte/short/ushort/int/uint/long) | `sbyte`…`long` | `attribute_val_num_integer.value_bigint` |
| Réel (float/double) | `float`/`double` | `attribute_val_num_float.value_double` |
| JSON | `json` | `attribute_val_json.value_json` |
| Valeur de référentiel | `lookup` | `attribute_val_lookup.lookup_item_id` |
| Référence vers une autre entité | `ref_entity` | `attribute_val_ref_entity.ref_entity_id` |

> Les **bornes natives** (ex. `byte` 0–255) sont vérifiées **dans l’application** avant d’écrire en base.

---

## 5) Génération de code C# (comment on s’en sert)

- **Nom de classe** = `entity_type.code`  
- **Noms de propriétés** = `attribute_def.code`  
- **Accesseur** par attribut (règle) :  
  - `is_private_set = false` → `public { get; set; }`  
  - `is_private_set = true`  → `public { get; private set; }`
- **Constructeur** : paramètres = attributs **obligatoires** ou **private-set**, **dans l’ordre `order_index`**, puis affectation des propriétés.

---

## 6) Exports CSV (1 fichier par type d’entité)
- **Colonnes** = attributs appliqués à l’entité, **triées par `order_index`** (puis par code si égalité).  
- Chaque colonne est “aplaties” depuis les tables de valeurs typées (on remonte le bon champ selon le `data_kind`).

---

## 7) Règles simples à retenir
- **`code` = nom C#** (classe/propriété). **`label`** = affichage UI.  
- Pour une paire (entité, attribut), il y a **exactement une** valeur dans **une seule** table typée.  
- Les **FK** sont en `BIGINT` (cohérent avec les PK).  
- `order_index` sert à **tout ordonner** (UI, CSV, constructeur). Utiliser des pas de **10** (10, 20, 30…).

---

## 8) Petit schéma mental (ASCII)

```
entity_type (ShipModel)
   │
   ├─ attribute_rule (applique les attributs à ShipModel)
   │      ├─ is_required / is_private_set / order_index
   │      └─ → attribute_def (Name, MaxSpeed, ...)
   │
   └─ entity (Corvette MK1, Explorer MK2, ...)
          └─ attribute_val (pivot par attribut appliqué)
                 ├─ attribute_val_text / _num_float / _num_integer / _bool / _json
                 ├─ attribute_val_lookup → lookup_item
                 └─ attribute_val_ref_entity → entity (référence forte)
```

Et pour les liens entre entités (M:N) :
```
entity_relation
   ├─ from_entity_id → entity
   ├─ to_entity_id   → entity
   └─ relation_type_id → lookup_item (groupe: relation_type)
```
