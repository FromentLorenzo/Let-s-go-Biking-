using System;
using System.Device.Location;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ServerSoap
{
    public class OpenStreetMapGeocodingService
    {
        private readonly HttpClient httpClient;
        private const string NominatimApiUrl = "https://nominatim.openstreetmap.org/search?format=json&q=";

        public OpenStreetMapGeocodingService()
        {
            this.httpClient = new HttpClient();
        }

        public double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var coord1 = new GeoCoordinate(lat1, lon1);
            var coord2 = new GeoCoordinate(lat2, lon2);
            return coord1.GetDistanceTo(coord2);
        }

        public async Task<(double Latitude, double Longitude)> GetCoordinatesAsync(string address)
        {
            try
            {
                string apiUrl = $"{NominatimApiUrl}{Uri.EscapeDataString(address)}";
                HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    JArray results = JArray.Parse(responseBody);

                    // Vérifiez si la réponse contient des résultats
                    if (results.Count > 0)
                    {
                        double latitude = (double)results[0]["lat"];
                        double longitude = (double)results[0]["lon"];

                        return (latitude, longitude);
                    }
                }

                // En cas d'échec ou d'absence de résultats
                Console.WriteLine($"Échec de la récupération des coordonnées pour l'adresse : {address}");
                return (0, 0); // Vous pouvez choisir un autre comportement par défaut si nécessaire
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la récupération des coordonnées : {ex.Message}");
                return (0, 0);
            }
        }
    }
}
