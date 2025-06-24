$KongAdminUrl = "http://localhost:8001"

function Reset-KongServices {
    # Supprimer toutes les routes
    $routes = Invoke-RestMethod -Method GET -Uri "$KongAdminUrl/routes"
    foreach ($route in $routes.data) {
        Write-Host "Suppression de la route: $($route.id)"
        Invoke-RestMethod -Method DELETE -Uri "$KongAdminUrl/routes/$($route.id)"
    }

    # Supprimer tous les services
    $services = Invoke-RestMethod -Method GET -Uri "$KongAdminUrl/services"
    foreach ($service in $services.data) {
        Write-Host "Suppression du service: $($service.name)"
        Invoke-RestMethod -Method DELETE -Uri "$KongAdminUrl/services/$($service.id)"
    }
}

function Create-Service {
    param(
        [string]$name,
        [string]$protocol,
        [string]$serviceHost,
        [int]$port,
        [string]$path,
        [string]$route
    )

    Write-Host "`nAjout du service $name -> $($protocol)://$($serviceHost):$($port)$($path) (route /$($route))"


    # Créer le service
    Invoke-RestMethod -Method POST -Uri "$KongAdminUrl/services" -Body @{
        name       = $name
        protocol   = $protocol
        host       = $serviceHost
        port       = $port
        path       = $path
        tls_verify = "false"
    } -ContentType "application/x-www-form-urlencoded"

    # Créer la route
    Invoke-RestMethod -Method POST -Uri "$KongAdminUrl/services/$name/routes" -Body @{
        "paths[]" = "/$route"
    } -ContentType "application/x-www-form-urlencoded"
}

Reset-KongServices

# Créer les services
Create-Service "produit-service"     "https" "host.docker.internal" 7198 "/api/v1/produits"     "produits"
Create-Service "vente-service"       "https"  "host.docker.internal" 7184 "/api/v1/ventes"       "ventes"
Create-Service "magasin-service"     "https"  "host.docker.internal" 7013 "/api/v1/magasins"     "magasins"
Create-Service "stock-service"       "https"  "host.docker.internal" 7185 ""                     "stocks"
Create-Service "rapport-service"     "https"  "host.docker.internal" 7214 "/api/v1/rapports"     "rapports"
Create-Service "performance-service" "https"  "host.docker.internal" 7044 "/api/v1/performances" "performances"
Create-Service "client-service"      "https"  "host.docker.internal" 7041 "/api/v1/clients"      "clients"
Create-Service "panier-service"      "https"  "host.docker.internal" 7019 "/api/v1/panier"       "panier"
Create-Service "commande-service"    "https"  "host.docker.internal" 7154 "/api/v1/commandes"    "commandes"

Write-Host " Tous les services et routes ont été créés avec succès !"
