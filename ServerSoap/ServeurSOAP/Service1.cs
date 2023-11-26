using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.Linq;
using ServeurSoap;

namespace ServeurSOAP
{
    public class Service1 : IService1
    {
        static readonly HttpClient client = new HttpClient();

        public async Task<string> getRoute(string depart, string arrivee)
        {
            string contrat = getContract(depart);
            Task<List<Station>> stationsTask = GetStationsAsync("e65de5172e58282b856fd72204eb35c710d1e336", contrat);

            List<Station> sortedStationsForDepart = await GetStationsByDistanceAsync(stationsTask, depart);
            Station closestStationDepart = sortedStationsForDepart.First();
            Console.WriteLine("premiere station depart: " + closestStationDepart.name);
            List<Station> sortedStationsForArrivee = await GetStationsByDistanceAsync(stationsTask, arrivee);

            if (sortedStationsForDepart != null && sortedStationsForDepart.Any() && sortedStationsForArrivee != null && sortedStationsForArrivee.Any())
            {
                // Récupérer la station la plus proche (première de la liste triée)
               
                Console.WriteLine($"La station la plus proche du départ est : {closestStationDepart.name}, Distance : {closestStationDepart.distanceToTheStation} meters");

                Station closestStationArrivee = sortedStationsForArrivee.First();
                Console.WriteLine($"La station la plus proche de l'arrivée est : {closestStationArrivee.name}, Distance : {closestStationArrivee.distanceToTheStation} meters");

                return $"depart: {closestStationDepart.name}, arrivee: {closestStationArrivee.name}";
            }

            Console.WriteLine("erreur");
            return null;
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
                    Console.WriteLine("Stations triées par distance croissante:");
                    foreach (var station in stations)
                    {
                        Console.WriteLine($"Station: {station.name}, Distance: {station.distanceToTheStation} meters");
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
                Console.WriteLine(depart.Substring(indexVirgule + 1).Trim());
                return depart.Substring(indexVirgule + 1).Trim();
            }

            // Aucune virgule trouvée, ou la virgule est à la fin de la chaîne
            return null;
        }

        static async Task<List<Station>> GetStationsAsync(string apiKey, string contract)
        {
            Console.WriteLine($"on rentre bien");
            HttpResponseMessage responseStations = await client.GetAsync($"https://api.jcdecaux.com/vls/v3/stations?contract={contract}&apiKey={apiKey}");

            // Vérifiez la réponse HTTP
            if (responseStations.IsSuccessStatusCode)
            {
                Console.WriteLine("Réponse HTTP réussie");

                // Récupérez le contenu de la réponse
                string responseBody = await responseStations.Content.ReadAsStringAsync();
                //Console.WriteLine($"Contenu de la réponse JSON avant désérialisation : {responseBody}");

                List<Station> stations = new List<Station>();

                try
                {
                    Console.WriteLine($"Stations");
                    JArray stationsArray = JArray.Parse(responseBody);

                    foreach (JToken stationToken in stationsArray)
                    {
                        // Désérialisez chaque objet de station individuellement
                        Station station = stationToken.ToObject<Station>();
                        stations.Add(station);
                        //Console.WriteLine("lat: " + station.position.latitude);
                        //Console.WriteLine("lon: " + station.position.longitude);
                    }
                    Console.WriteLine($"Stations: {stations.Count}");
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
                Console.WriteLine($"Échec de la requête HTTP avec le code : {responseStations.StatusCode}");
                // Gérer l'échec de la requête HTTP si nécessaire
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
    }

    public class Position
    {
        public double latitude { get; set; }
        public double longitude { get; set; }
    }

}
