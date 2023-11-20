using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Device.Location;
using System.Text.Json;

namespace routingServer
{
    internal class program
    {
        static readonly HttpClient client = new HttpClient();

        static async Task Main()
        {
            string apiKey = "e65de5172e58282b856fd72204eb35c710d1e336";

            // Assume you have the user's provided latitude and longitude
            double userLatitude = 48.8566; // Example latitude
            double userLongitude = 2.3522; // Example longitude
        }

        public static List<Station> GetSortedStations(string apiKey, double userLongitude, double userLatitude)
        {
            try
            {
                // Get stations for the specified contract
                List<Station> stations = GetStationsAsync(apiKey).Result;

                // Calculate distances to the stations and sort by distance
                stations.ForEach(station =>
                {
                    station.distanceToTheStation = CalculateDistance(userLatitude, userLongitude, station.position.lat, station.position.lng);
                });

                // Sort stations by distance
                stations.Sort((s1, s2) => s1.distanceToTheStation.CompareTo(s2.distanceToTheStation));

                return stations;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                return null;
            }
        }

        static async Task<List<Station>> GetStationsAsync(string apiKey)
        {
            HttpResponseMessage responseStations = await client.GetAsync($"https://api.jcdecaux.com/vls/v1/stations?apiKey={apiKey}");
            responseStations.EnsureSuccessStatusCode();
            string responseBody = await responseStations.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Station>>(responseBody);
        }

        static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {

            var coord1 = new GeoCoordinate(lat1, lon1);
            var coord2 = new GeoCoordinate(lat2, lon2);
            return coord1.GetDistanceTo(coord2);
        }
    }


}
