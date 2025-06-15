# ADR 004 – Choix de l’outil de test de charge : Apache JMeter

## Status

Acceptée

## Contexte

Pour valider la robustesse et la scalabilité de l’API, il est nécessaire d’effectuer des tests de charge réalistes, en simulant plusieurs scénarios simultanés (consultation de stock, génération de rapport, mise à jour de produit).

## Décision

J’ai choisi Apache JMeter comme outil principal de test de charge, car il est :

- Gratuit et open-source

- Compatible avec HTTP et APIs REST

- Facilement configurable avec des scénarios personnalisés

- Compatible avec des environnements CI/CD

## Conséquences

Possibilité d’automatiser les tests de charge dans la pipeline

Visualisation graphique de la montée en charge

Détection proactive des points de rupture et goulets d’étranglement
