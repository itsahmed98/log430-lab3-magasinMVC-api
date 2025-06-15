# ADR 005 – Mise en place d’un Load Balancer : NGINX

## Status

Acceptée

## Contexte

Le système évolue vers une architecture plus scalable, avec plusieurs instances du backend tournant en parallèle. Il est nécessaire d’équilibrer la charge entre ces instances et d’assurer une tolérance aux pannes.

## Décision

J’ai choisi NGINX comme répartiteur de charge (load balancer) pour les raisons suivantes :

- Léger, performant et largement utilisé

- Facile à configurer via Docker

- Supporte plusieurs stratégies de répartition : round-robin, least connections, etc.

- Compatible avec les containers Docker dans un environnement local

## Conséquences

Amélioration de la tolérance aux pannes : les requêtes sont redirigées si une instance échoue

Scalabilité horizontale : il est facile d’ajouter des instances derrière le proxy

Tests comparatifs possibles sur les stratégies de load balancing
