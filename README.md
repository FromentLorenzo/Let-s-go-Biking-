# Let-s-go-Biking-
Etudiants: Maurois Quentin, Froment Lorenzo

## Organisation du projet:
Le projet comporte 3 dossiers et 1 .bat pour lancer les serveurs.
Les 3 dossiers sont les suivants:
  - Un HeavyClient fait en java avec maven, dans lequel vous trouverez un .bat pour lancer le client
  - Un Proxy permettant d'effectuer des appels à JCDecaux et stocker les valeurs appelées redondantes
  - Un Serveur auquel le client fait appel et qui traite les différentes situtations possibles selon l'adresse de départ et d'arrivée

## Lancer le projet:
  - Attention, veillez bien à avoir le port 8090 ouvert pour que le service host des serveurs ne nécéssitent pas d'être run en admin
  - Lancez le premier .bat puis lancer le second situé dans le dossier HeavyClient.

## Utilisation du client:
-Dans la page qui s'est ouverte, commencez par déplacer la barre verticale vers la gauche à votre covenance pour ajuste l'espace où se trouveront les instructions
-Entrez une adresse de départ et d'arrivée sous le format: "adresse, ville"
-Cliquez sur "Trouver l'itinéraire"
-Lorsque le message: "Appuyez sur le bouton "Appeler MQ" pour obtenir l'itinéraire" s'affiche, le calcul d'itinéraire est terminé et vous pouvez donc appuyer sur appeler MQ pour afficher la prochaine étape
-Si rien ne s'est affiché, c'est seulement qu'un retour à la ligne s'est affiché, n'hésitez pas à rappuyer
-Vous avez la carte pour vous donner une idée du trajet à faire


## Fonctionnalitées implémentées:

- Implémentation d'un serveur c# avec un self-hosted
- Appel aux APIs : BingMap, OpenRouteService, OpenStreetMap, JCDecaux
- Serveur ActiveMQ pemetttant l'envoie et la réception des instructions au travers d'une queue
- Map sur le HeavyClient
- Serveur ProxyCache
