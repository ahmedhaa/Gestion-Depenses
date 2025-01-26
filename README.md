# Gestion-Depenses

## Description

Ce projet implémente une API REST pour la gestion des dépenses en utilisant .NET Core 6.0 et plusieurs technologies pour la gestion de la base de données, la sécurité, et la documentation de l'API. 

## Sécurité de l'API

- **Authentification** : Toutes les actions nécessitent un token JWT valide. Le jeton expire après 3 heures.
- **Utilisateur Préconfiguré** : Vous pouvez tester l'API avec l'utilisateur(Admin) suivant :
  - **Email** : "hitech@test.com"
  - **Mot de passe** : "Password123!"

## Technologies Utilisées

- **.NET Core 6** (Visual Studio 2022) : Framework principal du projet.
- **Entity Framework Core 6.0.0** : Gestion des entités et des migrations de la base de données.
- **SQLite** : Base de données légère pour stocker les informations des dépenses.
- **JWT** : Authentification sécurisée avec des tokens JSON Web.
- **ASP.NET Core Identity** : Gestion des utilisateurs, rôles et authentification.
- **Swagger** : Documentation de l'API pour faciliter l'intégration.
- **C#** : Langage de programmation utilisé.

## Initialisation de la Base de Données

Lors du premier démarrage, la base de données est automatiquement générée.

## Dépendances Installées

Les dépendances nécessaires pour le projet incluent :

- `Microsoft.EntityFrameworkCore`
- `Microsoft.EntityFrameworkCore.Sqlite`
- `Microsoft.AspNetCore.Identity.EntityFrameworkCore`
- `Microsoft.AspNetCore.Authentication.JwtBearer`
 ...

## Structure du Projet

Le projet est organisé comme suit :

- **Controllers** : Contient les endpoints REST pour l'authentification (AuthentificationController) et la gestion des dépenses (DepensesController).
- **Data** : Contient le `DepenseDbContext`, la classe de contexte représentant la connexion et le mappage entre l'application et la base de données.
- **Models** : Définit les entités du projet (Depense,typeDepense,Deplacement, Restaurant, User).
- **DTOs** : Définit les objets de transfert de données pour les entités (DepenseDto, DeplacementDto, RestaurantDto).
- **Migrations**: pour la gestion des migrations de la base de données
- **Services** : Contient les services pour l'initialisation des rôles (Admin, User) et des utilisateurs.
- **Program.cs** : Configuration principale de l'application qui contient :
  - **Middleware pour la Gestion Centralisée des Exceptions** : Capture et formate les erreurs globales en JSON.
  - **Gestion des Clés JWT** : Clés sécurisées pour signer et valider les tokens JWT.

## Fichier JSON de Configuration

Le projet utilise un fichier JSON pour stocker les configurations importantes telles que :

- **JWT** : La clé secrète utilisée pour signer les tokens JWT.
- **Chaîne de connexion de la base de données SQLite.**

## Configuration

- **Base de données** : Utilisation de SQLite pour la gestion légère des données.
- **Swagger** : Configuration pour générer la documentation de l'API.

## Architecture

Ce projet suit une architecture en couches

## Gestion des Dépenses via l'API

### API GET pour récupérer toutes les dépenses

- **Description** : Cette méthode permet de récupérer toutes les dépenses avec une pagination.
- **Paramètres** :
  - "page" : Le numéro de la page à récupérer.
  - "pageSize" : Le nombre d'éléments par page.
- **Réponse** : Liste des dépenses avec les informations suivantes : ID, Montant, Commentaire, Nature, Date, et des informations de pagination (total des éléments et nombre de pages).

### API GET pour récupérer une dépense spécifique par ID

- **Description** : Cette méthode permet de récupérer les informations d'une dépense spécifique par son ID.
- **Réponse** : Détails de la dépense : Montant, Commentaire, Nature, Date.

### Ajouter une dépense

- **Description** : Cette méthode permet d'ajouter une nouvelle dépense. L'ID et la Date de la dépense sont enregistrés automatiquement.
NB : Si vous choisissez Nature : 0 (Déplacement), le champ NombreInvités n'est pas pris en compte, 
                mais il doit être un entier ou null.
                Si vous choisissez Nature : 1 (Restaurant), le champ Distance n'est pas pris en compte
                mais il doit être un entier ou null. 
                Dans tous les cas, ces deux champs sont obligatoires.
- **Réponse** : Retourne la dépense ajoutée avec son ID généré
![image](https://github.com/user-attachments/assets/98db4625-6e7b-46fc-8e80-7a78fa730d01)




### Suppression d'une dépense par ID

- **Description** : Cette méthode permet de supprimer une dépense spécifique par son ID.
- **Réponse** : Message confirmant la suppression avec succès.

## Temps Consacré

Le développement, les tests et la configuration finale ont pris **1.5 jours**.
