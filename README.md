# TestFlow AI

Outil web intelligent d'automatisation et de gestion des tests pour systèmes embarqués, assisté par intelligence artificielle.

## Vue d'ensemble

TestFlow AI permet aux équipes de développement embarqué de :

- **Gérer des campagnes de tests** — organisation, planification et suivi des cycles de validation
- **Générer automatiquement des cas de test** — via un LLM local (Ollama) à partir des spécifications
- **Prédire les zones à risque** — analyse des historiques et priorisation intelligente des tests
- **Traçabilité complète** — lien entre exigences, cas de test et résultats

## Stack technique

| Couche | Technologie |
|---|---|
| Back-end | ASP.NET Core 8 (C#) |
| Front-end | React 18 + TypeScript + Vite |
| Base de données | SQL Server Express (local) |
| ORM | Entity Framework Core |
| IA / LLM | Ollama (modèles open source en local) |
| Tests | xUnit |
| Conteneurisation | Docker / Docker Compose |

## Structure du projet

```
testflow-ai/
├── src/
│   └── TestFlowAI.API/          # ASP.NET Core Web API
│       ├── Controllers/          # Endpoints REST
│       ├── Models/               # Entités de domaine
│       ├── Data/                 # DbContext & migrations EF Core
│       ├── Services/             # Logique métier & intégration Ollama
│       └── DTOs/                 # Data Transfer Objects
├── tests/
│   └── TestFlowAI.Tests/         # Tests unitaires xUnit
├── frontend/                     # Application React (Vite + TypeScript)
│   ├── src/
│   │   ├── components/           # Composants React réutilisables
│   │   ├── pages/                # Pages de l'application
│   │   ├── services/             # Appels API (Axios)
│   │   ├── store/                # État global (Zustand)
│   │   └── types/                # Types TypeScript partagés
│   └── public/
├── docker-compose.yml            # SQL Server Express local
├── TestFlowAI.sln
└── README.md
```

## Prérequis

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 20+](https://nodejs.org/)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [Ollama](https://ollama.ai/) avec un modèle installé (ex: `ollama pull llama3`)

## Démarrage rapide

### 1. Démarrer SQL Server (Docker)

```bash
docker-compose up -d
```

### 2. Lancer le back-end

```bash
cd src/TestFlowAI.API
dotnet restore
dotnet ef database update
dotnet run
```

L'API est disponible sur `http://localhost:5000` — Swagger UI sur `http://localhost:5000/swagger`

### 3. Lancer le front-end

```bash
cd frontend
npm install
npm run dev
```

L'application est disponible sur `http://localhost:5173`

### 4. Démarrer Ollama

```bash
ollama serve
ollama pull llama3
```

## Fonctionnalités principales

### Campagnes de tests
- Créer et configurer des campagnes de validation
- Associer des exigences systèmes et des composants embarqués
- Suivre l'avancement et les taux de couverture

### Génération IA de cas de test
- Décrire un composant ou une fonction en langage naturel
- Le LLM (via Ollama) génère des cas de test structurés
- Revue et intégration dans les campagnes existantes

### Prédiction des zones à risque
- Analyse des historiques de tests et des taux d'échec
- Score de risque par module ou fonctionnalité
- Priorisation automatique de l'ordre d'exécution

## Licence

MIT
