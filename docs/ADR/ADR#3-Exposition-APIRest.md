# ADR 003 – Exposition d’une API RESTful

## Status

Acceptée

## Contexte

Afin de préparer le système à des évolutions futures (interface web/mobile, microservices) et d'assurer une séparation claire entre la logique métier et la présentation, il est nécessaire d'exposer les fonctionnalités via une API RESTful.

## Décision

J’ai décidé d’ajouter une couche API RESTful au projet. Cette couche expose les opérations métier sous forme de routes HTTP standardisées, en respectant les principes REST :

- Architecture sans état

- Utilisation des verbes HTTP (GET, POST, PUT, DELETE, etc.)

- Structure claire des routes : /api/ressource, /api/ressource/{id}, etc.

- Documentation via Swagger/OpenAPI

## Conséquences

Permet l’intégration d’un front-end web ou mobile à l’avenir

Facilite les tests via Swagger/Postman

Sépare proprement la logique métier et l’interface utilisateur

Prépare la transition vers des microservices si nécessaire
