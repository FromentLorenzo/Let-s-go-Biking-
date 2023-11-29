using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.Linq;
using ServeurSoap;
using ServeurSOAP.ServiceReference1;

namespace ServeurSOAP
{
    public class Service1 : IService1
    {
        static readonly HttpClient client = new HttpClient();
        static readonly ProxyClient client2 = new ProxyClient();


        public async Task<string> getRoute(string depart, string arrivee)
        {
            string contrat = getContract(depart);
            Task<List<Station>> stationsTask = GetStationsAsync("e65de5172e58282b856fd72204eb35c710d1e336", contrat);

            List<Station> sortedStationsForDepart = await GetStationsByDistanceAsync(stationsTask, depart);
            Station closestStationDepart= null;
            foreach(var station in sortedStationsForDepart)
            {
                if (station.totalStands.availabilities.bikes > 0)
                {
                    closestStationDepart = sortedStationsForDepart.First();
                    break;
                }
            }
            
            //Console.WriteLine("premiere station depart: " + closestStationDepart.name);
            List<Station> sortedStationsForArrivee = await GetStationsByDistanceAsync(stationsTask, arrivee);

            if (sortedStationsForDepart != null && sortedStationsForDepart.Any() && sortedStationsForArrivee != null && sortedStationsForArrivee.Any())
            {
                // Récupérer la station la plus proche (première de la liste triée)

                //Console.WriteLine($"La station la plus proche du départ est : {closestStationDepart.name}, Distance : {closestStationDepart.distanceToTheStation} meters");
                Station closestStationArrivee = null;
                foreach(var station in sortedStationsForArrivee)
                {
                    if(station.totalStands.availabilities.stands > 0)
                    {
                        closestStationArrivee = sortedStationsForArrivee.First();
                        break;
                    }

                }
                 
                //Console.WriteLine($"La station la plus proche de l'arrivée est : {closestStationArrivee.name}, Distance : {closestStationArrivee.distanceToTheStation} meters");
                
                Task<string> howToMove = bikingOrWalking(closestStationDepart, closestStationArrivee, depart, arrivee);
                //Console.WriteLine(howToMove.Result);
                string howToMoveResult = await howToMove;
                if (howToMoveResult == "Bicycling")
                {
                    OpenStreetMapGeocodingService geocodingService = new OpenStreetMapGeocodingService();
                    bing bing = new bing();
                    OpenRouteService routeService = new OpenRouteService();
                    var coordDepar = await geocodingService.GetCoordinatesAsync(depart);
                    var coordArriv = await geocodingService.GetCoordinatesAsync(arrivee);

                    // Instructions du départ à la première station
                    // ...

                    // Instructions du départ à la première station
                    Task<List<string>> instructionsDepartStaionD = bing.GetInstructions("Walking", coordDepar.Latitude, coordDepar.Longitude, closestStationDepart.position.latitude, closestStationDepart.position.longitude);

                    // Instructions de la première station à la deuxième station
                    Task<List<string>> instructionsStationDStationA = routeService.GetInstructions("cycling-regular", closestStationDepart.position.latitude, closestStationDepart.position.longitude, closestStationArrivee.position.latitude, closestStationArrivee.position.longitude);

                    // Instructions de la deuxième station à la destination
                    Task<List<string>> instructionsStationAArrivee = bing.GetInstructions("Walking", closestStationArrivee.position.latitude, closestStationArrivee.position.longitude, coordArriv.Latitude, coordArriv.Longitude);

                    // Attendre l'achèvement de toutes les tâches asynchrones
                    await Task.WhenAll(instructionsDepartStaionD, instructionsStationDStationA, instructionsStationAArrivee);

                    // Afficher chaque ensemble d'instructions sur une nouvelle ligne
                    Console.WriteLine("You are going by Bike, let's go to the first Station:");
                    Console.WriteLine(string.Join("\n", instructionsDepartStaionD.Result));

                    Console.WriteLine("\nNow this is the bike itinerary:");
                    Console.WriteLine(string.Join("\n", instructionsStationDStationA.Result));

                    Console.WriteLine("\nAnd now you can go to your destination by following this:");
                    Console.WriteLine(string.Join("\n", instructionsStationAArrivee.Result));

                    // Concaténer les instructions dans une seule chaîne pour le retour
                    string result = "You are going by Bike, let's go to the first Station:\n\n" +
                                    string.Join("\n", instructionsDepartStaionD.Result) +
                                    "\n\nNow this is the bike itinerary:\n\n" +
                                    string.Join("\n", instructionsStationDStationA.Result) +
                                    "\n\nAnd now you can go to your destination by following this:\n\n" +
                                    string.Join("\n", instructionsStationAArrivee.Result);

                    return result;

                }

                if (howToMoveResult == "Walking")
                {
                    OpenStreetMapGeocodingService geocodingService = new OpenStreetMapGeocodingService();
                    bing bing=new bing();
                    var coordDepar = await geocodingService.GetCoordinatesAsync(depart);
                    var coordArriv = await geocodingService.GetCoordinatesAsync(arrivee);
                    Task<List<string>> instructions = bing.GetInstructions(howToMoveResult, coordDepar.Latitude, coordDepar.Longitude, coordArriv.Latitude, coordArriv.Longitude);
                    List<string> instructionResult = await instructions;

                    // Afficher chaque instruction sur une nouvelle ligne
                    foreach (string instruction in instructionResult)
                    {
                        Console.WriteLine(instruction);
                    }

                    // Concaténer les instructions dans une seule chaîne
                    string instructionsAsString = string.Join("\n", instructionResult);

                    return "You should probably go by walking, it's faster\n" + instructionsAsString;
                }



            }

            Console.WriteLine("erreur");
            return null;
        }



        public async Task<string> bikingOrWalking(Station stationDeDepart, Station stationDArrivee, string depart, string arrivee )
        {
            OpenStreetMapGeocodingService geocodingService = new OpenStreetMapGeocodingService();
            var coordDepar = await geocodingService.GetCoordinatesAsync(depart);
            var coordArriv = await geocodingService.GetCoordinatesAsync(arrivee);
            double distanceEntreDepartEtArrivee=geocodingService.CalculateDistance(coordDepar.Latitude,coordDepar.Longitude,coordArriv.Latitude,coordArriv.Longitude);
            double distanceEntreStations = geocodingService.CalculateDistance(stationDeDepart.position.latitude, stationDeDepart.position.longitude, stationDArrivee.position.latitude, stationDArrivee.position.longitude);
            double distanceEntreDepartEtStationDepart = geocodingService.CalculateDistance(coordDepar.Latitude, coordDepar.Longitude, stationDeDepart.position.latitude, stationDeDepart.position.longitude);
            double distanceEntreArriveeEtStationArrivee = geocodingService.CalculateDistance(coordArriv.Latitude, coordArriv.Longitude, stationDArrivee.position.latitude, stationDArrivee.position.longitude);
            if(distanceEntreDepartEtArrivee<distanceEntreArriveeEtStationArrivee||
                distanceEntreDepartEtArrivee<distanceEntreDepartEtStationDepart||
                distanceEntreDepartEtArrivee<distanceEntreArriveeEtStationArrivee+distanceEntreDepartEtStationDepart||
                distanceEntreDepartEtArrivee < distanceEntreArriveeEtStationArrivee*2 ||
                distanceEntreDepartEtArrivee < distanceEntreDepartEtStationDepart * 2)
            {
                return "Walking";
            }
            else
            {
                return "Bicycling";
            }

        }


        public async Task<List<Station>> GetStationsByDistanceAsync(Task<List<Station>> stationTask, string depart)
        {
            try
            {
                // Attendez la complétion de la tâche qui récupère la liste des stations
                List<Station> stations = await stationTask;

                // Vérifiez si la tâche s'est terminée correctement
                if (stationTask.Status == TaskStatus.RanToCompletion && stations.Any())
                {
                    OpenStreetMapGeocodingService geocodingService = new OpenStreetMapGeocodingService();
                    var coordinates = await geocodingService.GetCoordinatesAsync(depart);
                    //Console.WriteLine(coordinates.ToString());

                    stations.ForEach(station =>
                    {
                        station.distanceToTheStation = geocodingService.CalculateDistance(coordinates.Latitude, coordinates.Longitude, station.position.latitude, station.position.longitude);
                    });

                    // Trier les stations par distance croissante
                    stations.Sort((s1, s2) => s1.distanceToTheStation.CompareTo(s2.distanceToTheStation));

                    // Afficher les stations triées
                    //Console.WriteLine("Stations triées par distance croissante:");
                    foreach (var station in stations)
                    {
                        //Console.WriteLine($"Station: {station.name}, Distance: {station.distanceToTheStation} meters");
                    }

                    return stations;
                }
                else
                {
                    Console.WriteLine("La tâche de récupération des stations a échoué ou la liste de stations est vide.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Une erreur s'est produite : {ex.Message}");
            }

            return null;
        }

        public string getContract(string depart)
        {
            int indexVirgule = depart.IndexOf(',');

            // Vérifier si la virgule a été trouvée et extraire le nom de la ville
            if (indexVirgule != -1 && indexVirgule < depart.Length - 1)
            {
                //Console.WriteLine(depart.Substring(indexVirgule + 1).Trim());
                return depart.Substring(indexVirgule + 1).Trim();
            }

            // Aucune virgule trouvée, ou la virgule est à la fin de la chaîne
            return null;
        }

        static async Task<List<Station>> GetStationsAsync(string apiKey, string contract)
        {
            //Console.WriteLine("tout va bien");
            string allStations = client2.getAllStationsOfAContract(contract);

            if (!string.IsNullOrEmpty(allStations))
            {
                try
                {
                    //Console.WriteLine("Réponse HTTP réussie");
                    //Console.WriteLine($"Contenu de la réponse JSON avant désérialisation : {allStations}");

                    List<Station> stations = JsonConvert.DeserializeObject<List<Station>>(allStations);
                    //Console.WriteLine($"Stations: {stations.Count}");

                    return stations;
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Erreur de désérialisation JSON : {ex.Message}");
                    throw; // Lancer à nouveau l'exception pour une gestion ultérieure si nécessaire
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur inattendue : {ex.Message}");
                    throw; // Lancer à nouveau l'exception pour une gestion ultérieure si nécessaire
                }
            }
            else
            {
                Console.WriteLine("La requête HTTP a échoué ou la réponse est vide.");
                return null; // Ou une autre valeur par défaut en cas d'échec
            }
        }
    }

        public class Station
    {
        public int number { get; set; }
        public string contract_name { get; set; }
        public Position position { get; set; }
        public string name { get; set; }
        public double distanceToTheStation { get; set; }
        public TotalStands totalStands { get; set; }
    }
    public class TotalStands
    {
        public Availabilities availabilities { get; set; }
        public int capacity { get; set; }
    }
    public class Availabilities
    {
        public int bikes { get; set; }
        public int stands { get; set; }
    }

    public class Position
    {
        public double latitude { get; set; }
        public double longitude { get; set; }
    }

}
