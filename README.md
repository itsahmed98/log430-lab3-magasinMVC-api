# LABORATOIRE 3 — LOG430 | MagasinCentral à 3-couches

## Répo GitHub (public)

- https://github.com/itsahmed98/log430-lab0
- https://github.com/itsahmed98/log430-lab1
- https://github.com/itsahmed98/log430-lab2-mvc
- https://github.com/itsahmed98/log430-lab3-magasinMVC-api.git

---

## Brève description de l’application

Ce projet est une application Web (.NET 8) qui étend le fonctionnement de plusieurs caisses d’un commerce. Cette application offre une gestion de plusieurs magasin à partir d'un magasin central

L'application est monolithique et suit une architecture en couche 3-tier, mais ce qui est de plus dans ce laboratoire est l'exposition d'une API pour permettre l'intéropératbilité avec les clients externes.

---

## Cas d'utilisation du système

| Id  | Fonction                                                         |
| --- | ---------------------------------------------------------------- |
| UC1 | Générer un rapport consolidé des ventes                          |
| UC2 | Consulter le stock d'un magasin                                  |
| UC3 | Visualiser les performances des magasins dans un tableau de bord |
| UC4 | Mettre à jour les produits depuis la maison mère                 |

---

# Guide pour l'API

Quand on lance l'application, accèder l'api dans: /swagger

## Authentification

**TOUS les routes sont sécurisés. Il faut un bearer token pour y accèder. Pour cela, il faut enregistrer et ensuite login avec ce compte. Quand c'est faut, vous allez recevoir un bearer token. Ce token doit etre utilisé dans les requetes pour avoir accès.**

- POST /api/v1/auth/login

exemple de requete:
{
"email": "string",
"password": "string"
}

- POST /api/v1/auth/register

{
"email": "string",
"password": "string"
}

## Performances

- GET /api/v1/performances

## Produit

- GET /api/v1/produits
- GET /api/v1/produits/{produitId}
- PUT //api/v1/produits/{produitId}

  Exemple de requete:
  test avec:
  {
  "produitId": 3,
  "nom": "Clé USB 32 Go",
  "categorie": "Électronique",
  "prix": 15.00,
  "description": "Clé USB 32 Go avec protection améliorée333"
  }

## Rapport

- GET /api/v1/rapports

## Stock

- GET /api/v1/stock

## Suite de tests

Le projet contient un dossier `MagasinCentral.Tests` avec des tests unitaires. (Voir Structure du projet).

### Pour les exécuter :

```bash
cd MagasinCentral.Tests
dotnet test

```

## Structure du projet

```plaintext

log430-lab3-magasinmvc-api/
├── MagasinCentral/
│ ├── Program.cs
  ├── Api/
    ├── Controllers
        ├── AuthController.cs
        ├── PerformanceController.cs
        ├── ProduitController.cs
        ├── RapportController.cs
        ├── StockController.cs
│ ├── Models/
│ ├── Data/
│ ├── Services/
│ └── Migrations/
├── client.Tests/
│ ├── ProduitServiceTests.cs
│ ├── VenteServiceTests.cs
│ └── RetourServiceTests.cs
├── docs/
│ ├── ADR/
│ ├── UML/
│ ├── BesoinsDuClient.md
│ └── Cas-utilisations.md
├── Dockerfile
├── docker-compose.yml
├── .github/
│ └── workflows/
│ └── ci.yml
└── README.md
```

---

## Étapes d’installation et d’exécution

### 1. Cloner le dépôt et aller dans le fichier racine

    - git clone https://github.com/itsahmed98/log430-lab3-magasinMVC-api.git
    - cd log430-lab3-magasinMVC-ap

### 2. Lancer l'application avec docker compose

    - docker compose up --build -d

---

## Image Docker Hub

Les images sont disponible ici: https://hub.docker.com/u/ahmedsherif98

pour récupèrer une imgage - docker pull ahmedsherif98/magasincentral-mvc-api:latest

## 🚀 CI/CD — Pipeline

- https://github.com/itsahmed98/log430-lab3-magasinMVC-api/actions

Le pipeline CI/CD :

1. Restaure les dépendances
2. Vérifie la mise en forme du code (Linting)
3. Lance les tests unitaires (avec xunit)
4. Construit l’image Docker
5. Publie l’image sur Docker Hub (avec un tag par defaut "latest")
6. Deploiement en prod

## Auteur

Ahmed Akram Sherif
Étudiant au baccalauréat en génie logiciel
Cours : LOG430 — Été 2025


# Dockerfile template

# === Step 1: Build stage ===
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ["{PROJECT_FOLDER}/{PROJECT_NAME}.csproj", "{PROJECT_FOLDER}/"]
RUN dotnet restore "{PROJECT_FOLDER}/{PROJECT_NAME}.csproj"

# Copy everything and build
COPY . .
WORKDIR "/src/{PROJECT_FOLDER}"
RUN dotnet publish "{PROJECT_NAME}.csproj" -c Release -o /app/publish

# === Step 2: Runtime stage ===
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "{PROJECT_NAME}.dll"]
