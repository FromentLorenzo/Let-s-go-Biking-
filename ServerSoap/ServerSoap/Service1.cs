using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using ServerSoap;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.Linq;

public class Service1 : IService1
{
    static readonly HttpClient client = new HttpClient();

    public string getRoute(string depart, string arrivee)
    {
        string contrat = getContract(depart);
        Task<List<Station>> s = GetStationsAsync("e65de5172e58282b856fd72204eb35c710d1e336", contrat);
        Task<Station> closestStationTask = GetClosestStation(s, depart);
        Station closestStation = closestStationTask.Result;
        return contrat;
    }

    public async Task<Station> GetClosestStation(Task<List<Station>> s, string depart)
    {
        // Attendez la complétion de la tâche qui récupère la liste des stations
        await s;

        // Vérifiez si la tâche s'est terminée correctement
        if (s.Status == TaskStatus.RanToCompletion)
        {
            List<Station> stations = s.Result;

            // Vérifiez si la liste de stations n'est pas vide
            if (stations.Count > 0)
            {
                OpenStreetMapGeocodingService geocodingService = new OpenStreetMapGeocodingService();
                var coordinates = await geocodingService.GetCoordinatesAsync(depart);

                stations.ForEach(station =>
                {
                    station.distanceToTheStation = geocodingService.CalculateDistance(coordinates.Latitude, coordinates.Longitude, station.position.lat, station.position.lng);
                });

                // Sort stations by distance
                stations.Sort((s1, s2) => s1.distanceToTheStation.CompareTo(s2.distanceToTheStation));
                return stations.First();
            }
        }
        return null;
    }

    public string getContract(string depart)
    {
        int indexVirgule = depart.IndexOf(',');

        // Vérifier si la virgule a été trouvée et extraire le nom de la ville
        if (indexVirgule != -1 && indexVirgule < depart.Length - 1)
        {
            return depart.Substring(indexVirgule + 1).Trim();
        }

        // Aucune virgule trouvée, ou la virgule est à la fin de la chaîne
        return null;
    }

    static async Task<List<Station>> GetStationsAsync(string apiKey, string contract)
    {
        HttpResponseMessage responseStations = await client.GetAsync($"https://api.jcdecaux.com/vls/v3/stations?contract={contract}&apiKey={apiKey}");
        responseStations.EnsureSuccessStatusCode();
        string responseBody = await responseStations.Content.ReadAsStringAsync();

        List<Station> stations = new List<Station>();

        try
        {
            JArray stationsArray = JArray.Parse(responseBody);

            foreach (JToken stationToken in stationsArray)
            {
                // Désérialisez chaque objet de station individuellement
                Station station = stationToken.ToObject<Station>();
                stations.Add(station);
            }

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
    public double lat { get; set; }
    public double lng { get; set; }
}
