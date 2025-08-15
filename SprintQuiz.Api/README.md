# SprintQuiz API - Backend .NET 8

## 📋 Description

SprintQuiz est une plateforme de révision interactive organisée par sprints, modules et cours. Cette API backend fournit tous les services nécessaires pour gérer les quiz QCM, les questions-réponses type flashcards, et le suivi de progression des utilisateurs. Cette version inclut désormais un espace profil utilisateur complet et une gestion des permissions basée sur les rôles (Admin/Étudiant) via JWT.

## 🏗️ Architecture

L'application suit une architecture en couches avec le pattern Repository/Service/Controller :

```
SprintQuiz.Api/
├── Controllers/         → Contrôleurs REST API
├── Services/           → Logique métier
├── Repositories/       → Accès aux données
├── Models/            → Entités EF Core
├── DTOs/              → Data Transfer Objects
├── Mappings/          → Profils AutoMapper
├── Data/              → DbContext et configuration
├── Extensions/        → Extensions de services
├── Middleware/        → Middleware personnalisés
└── appsettings.json   → Configuration
```

## 🚀 Démarrage rapide

### Prérequis

- .NET 8 SDK
- PostgreSQL 13+
- Visual Studio 2022 ou VS Code

### Installation

1. **Cloner le projet**
   ```bash
   git clone <repository-url>
   cd SprintQuiz.Api
   ```

2. **Configurer la base de données**
   
   Modifier la chaîne de connexion dans `appsettings.json` :
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Database=SprintQuizDb;Username=postgres;Password=your_password"
     },
     "JwtSettings": {
       "SecretKey": "your_super_secret_key_here_minimum_32_characters",
       "Issuer": "SprintQuiz.Api",
       "Audience": "SprintQuiz.Client",
       "ExpirationInMinutes": 60
     }
   }
   ```
   Assurez-vous que `SecretKey` est une chaîne longue et complexe (minimum 32 caractères).

3. **Installer les dépendances**
   ```bash
   dotnet restore
   ```

4. **Créer et appliquer les migrations**
   ```bash
   dotnet ef migrations add InitialCreate --project SprintQuiz.Api
   dotnet ef database update --project SprintQuiz.Api
   ```
   *(Note: Si vous avez déjà exécuté `InitialCreate`, vous devrez peut-être créer une nouvelle migration pour les changements du modèle `Utilisateur` : `dotnet ef migrations add AddPhotoUrlToUser --project SprintQuiz.Api`)*

5. **Lancer l'application**
   ```bash
   dotnet run
   ```

L'API sera accessible sur `https://localhost:7000` avec Swagger UI à la racine.

## 📊 Modèle de données

### Entités pédagogiques
- **Sprint** : Ensemble de modules (ex: Sprint 1 à Sprint 9)
- **Module** : Ensemble cohérent de cours (ex: "Fondamentaux réseaux")
- **Cours** : Unité de base (ex: "La couche physique")

### Entités Quiz (QCM)
- **Quiz** : Container de questions QCM
- **QCMQuestion** : Question à choix multiple
- **QCMOption** : Option de réponse

### Entités Questions-Réponses
- **QAQuestion** : Question ouverte avec réponse (flashcards)

### Entités utilisateur et suivi
- **Utilisateur** : Compte utilisateur (admin/étudiant), inclut `PhotoUrl`
- **TentativeQuiz** : Tentative de quiz avec score
- **ConsultationQA** : Consultation de question-réponse
- **ProgressionUtilisateur** : Suivi de progression par niveau
- **StatistiquesGlobales** : Statistiques agrégées par utilisateur

## 🔌 Endpoints principaux

### Authentification
- `POST /api/utilisateur/login` - Authentifie un utilisateur et renvoie un JWT.

### Profil Utilisateur (nécessite authentification)
- `GET /api/profil` - Récupère les informations du profil de l'utilisateur connecté (inclut stats, progression, dernières activités).
- `PUT /api/profil` - Met à jour le nom, l'email de l'utilisateur connecté.
- `POST /api/profil/photo` - Upload une photo de profil (base64).
- `PUT /api/profil/password` - Change le mot de passe de l'utilisateur connecté.

### Sprints
- `GET /api/sprint` - Liste tous les sprints (Public)
- `GET /api/sprint/{id}` - Détails d'un sprint (Public)
- `GET /api/sprint/{id}/modules` - Sprint avec ses modules (Public)
- `POST /api/sprint` - Créer un sprint (Admin)
- `PUT /api/sprint/{id}` - Modifier un sprint (Admin)
- `DELETE /api/sprint/{id}` - Supprimer un sprint (Admin)

### Modules
- `GET /api/module` - Liste tous les modules (Public)
- `GET /api/module/{id}` - Détails d'un module (Public)
- `GET /api/module/{id}/cours` - Module avec ses cours (Public)
- `GET /api/module/sprint/{sprintId}` - Modules par sprint (Public)
- `POST /api/module` - Créer un module (Admin)
- `PUT /api/module/{id}` - Modifier un module (Admin)
- `DELETE /api/module/{id}` - Supprimer un module (Admin)

### Cours
- `GET /api/cours` - Liste tous les cours (Public)
- `GET /api/cours/{id}` - Détails d'un cours (Public)
- `GET /api/cours/module/{moduleId}` - Cours par module (Public)
- `POST /api/cours` - Créer un cours (Admin)
- `PUT /api/cours/{id}` - Modifier un cours (Admin)
- `DELETE /api/cours/{id}` - Supprimer un cours (Admin)

### Quiz
- `GET /api/quiz` - Liste tous les quiz (Public)
- `GET /api/quiz/{id}` - Détails d'un quiz (Public)
- `GET /api/quiz/{id}/questions` - Quiz avec questions (Authentifié)
- `GET /api/quiz/niveau/{niveau}/{niveauId}` - Quiz par niveau (Public)
- `POST /api/quiz` - Créer un quiz (Admin)
- `PUT /api/quiz/{id}` - Modifier un quiz (Admin)
- `DELETE /api/quiz/{id}` - Supprimer un quiz (Admin)
- `POST /api/quiz/submit` - Soumettre une tentative (Authentifié)
- `GET /api/quiz/mes-tentatives` - Tentatives de l'utilisateur connecté (Authentifié)
- `GET /api/quiz/utilisateur/{id}/tentatives` - Tentatives d'un utilisateur spécifique (Authentifié, Admin ou propre utilisateur)

### Questions-Réponses
- `GET /api/qaquestion` - Liste toutes les questions-réponses (Public)
- `GET /api/qaquestion/{id}` - Détails d'une question-réponse (Public)
- `GET /api/qaquestion/niveau/{niveau}/{niveauId}` - Questions par niveau (Public)
- `GET /api/qaquestion/ma-revision/niveau/{niveau}/{niveauId}` - Questions pour révision de l'utilisateur connecté (Authentifié)
- `GET /api/qaquestion/revision/{userId}/niveau/{niveau}/{niveauId}` - Questions pour révision d'un utilisateur spécifique (Authentifié, Admin ou propre utilisateur)
- `POST /api/qaquestion` - Créer une question-réponse (Admin)
- `PUT /api/qaquestion/{id}` - Modifier une question-réponse (Admin)
- `DELETE /api/qaquestion/{id}` - Supprimer une question-réponse (Admin)
- `POST /api/qaquestion/consult` - Enregistrer une consultation (Authentifié)
- `GET /api/qaquestion/mes-consultations` - Consultations de l'utilisateur connecté (Authentifié)
- `GET /api/qaquestion/utilisateur/{id}/consultations` - Consultations d'un utilisateur spécifique (Authentifié, Admin ou propre utilisateur)

### Utilisateurs
- `GET /api/utilisateur` - Liste tous les utilisateurs (Admin)
- `GET /api/utilisateur/{id}` - Détails d'un utilisateur (Authentifié, Admin ou propre utilisateur)
- `POST /api/utilisateur` - Créer un utilisateur (Admin)
- `PUT /api/utilisateur/{id}` - Modifier un utilisateur (Authentifié, Admin ou propre utilisateur)
- `DELETE /api/utilisateur/{id}` - Supprimer un utilisateur (Admin)
- `GET /api/utilisateur/mes-statistiques` - Statistiques de l'utilisateur connecté (Authentifié)
- `GET /api/utilisateur/{id}/statistiques` - Statistiques d'un utilisateur spécifique (Authentifié, Admin ou propre utilisateur)
- `GET /api/utilisateur/ma-progression` - Progression de l'utilisateur connecté (Authentifié)
- `GET /api/utilisateur/{id}/progression` - Progression d'un utilisateur spécifique (Authentifié, Admin ou propre utilisateur)

## 🔧 Configuration

### Base de données
L'application utilise PostgreSQL avec Entity Framework Core. Les migrations sont dans le dossier `Migrations/`.

### JWT Authentication
Le token JWT est généré lors de la connexion et doit être inclus dans l'en-tête `Authorization` sous la forme `Bearer <token>` pour les endpoints protégés.

### CORS
Configuré pour accepter toutes les origines en développement. À restreindre en production.

### Logging
Configuré avec les providers Console et Debug. Logs détaillés pour EF Core en développement.

### Swagger
Documentation API automatique disponible à la racine en développement, avec support pour l'authentification JWT.

### Fichiers statiques
Le dossier `wwwroot/uploads/profiles` est utilisé pour stocker les photos de profil uploadées.

## 🔐 Sécurité

### JWT
- Configuration dans `appsettings.json` (`JwtSettings`)
- Middleware d'authentification configuré dans `Program.cs`
- `ITokenService` pour la génération des tokens

### Autorisation basée sur les rôles
- Utilisation des attributs `[Authorize]` et `[Authorize(Roles = "Admin")]` sur les contrôleurs et les actions.
- Les rôles sont `Admin` et `Etudiant`.

### Validation
- Validation des modèles avec Data Annotations
- Middleware de gestion d'erreurs personnalisé

## 🧪 Tests

Structure recommandée pour les tests :
```
SprintQuiz.Tests/
├── Unit/              → Tests unitaires
├── Integration/       → Tests d'intégration
└── E2E/              → Tests end-to-end
```

## 📈 Workflows supportés

### Création de Quiz
1. Choix du contexte (cours/module/sprint)
2. Sélection de l'entité
3. Création du quiz avec métadonnées
4. Ajout des questions et options
5. Validation et enregistrement

### Révision Quiz
1. Sélection du quiz
2. Réponse aux questions
3. Soumission et correction
4. Affichage des résultats
5. Mise à jour de la progression

### Révision Questions-Réponses
1. Sélection du niveau de révision
2. Affichage des questions (flashcards)
3. Auto-évaluation (compris/pas compris)
4. Navigation entre les cartes
5. Mise à jour de la progression

### Gestion de Profil
1. Récupération des informations de profil (nom, email, photo, stats, progression, activités)
2. Mise à jour des informations de profil
3. Upload de photo de profil
4. Changement de mot de passe

## 🚀 Déploiement

### Variables d'environnement
- `ConnectionStrings__DefaultConnection` : Chaîne de connexion DB
- `JwtSettings__SecretKey` : Clé secrète JWT
- `ASPNETCORE_ENVIRONMENT` : Environnement (Development/Production)

### Docker (Recommandé)
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["SprintQuiz.Api.csproj", "."]
RUN dotnet restore
COPY . .
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SprintQuiz.Api.dll"]
```

## 📝 Contribution

1. Fork le projet
2. Créer une branche feature (`git checkout -b feature/AmazingFeature`)
3. Commit les changements (`git commit -m 'Add AmazingFeature'`)
4. Push vers la branche (`git push origin feature/AmazingFeature`)
5. Ouvrir une Pull Request

## 📄 Licence

Ce projet est sous licence MIT. Voir le fichier `LICENSE` pour plus de détails.

## 👥 Équipe

- **Backend** : Architecture .NET 8 + PostgreSQL
- **Frontend** : Angular 19 (projet séparé)
- **DevOps** : Docker + CI/CD

## 🔗 Liens utiles

- [Documentation .NET 8](https://docs.microsoft.com/en-us/dotnet/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [AutoMapper](https://automapper.org/)
- [Swagger/OpenAPI](https://swagger.io/)
- [JWT Authentication in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/jwt-bearer?view=aspnetcore-8.0)


