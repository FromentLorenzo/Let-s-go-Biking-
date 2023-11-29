using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ServeurSOAP;

namespace ServeurSoap
{
    public class OpenStreetMapGeocodingService 
    {
        private readonly HttpClient httpClient;
        private const string NominatimApiUrl = "https://nominatim.openstreetmap.org/search?format=json&q=";

        public OpenStreetMapGeocodingService()
        {
            this.httpClient = new HttpClient();
        }

 
        public double CalculateDistance(double lat2, double lon2, double lat1, double lon1)
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

                // Ajoutez un en-tête User-Agent à la requête
                httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:97.0) Gecko/20100101 Firefox/97.0");

                //Console.WriteLine($"Trying to get coordinates for address: {address}");

                using (HttpResponseMessage response = await httpClient.GetAsync(apiUrl))
                {
                    //Console.WriteLine("Request made");

                    if (response.IsSuccessStatusCode)
                    {
                        //Console.WriteLine("Response is successful");
                        string responseBody = await response.Content.ReadAsStringAsync();
                        JArray results = JArray.Parse(responseBody);

                        if (results.Count > 0)
                        {
                            
                            double latitude = (double)results[0]["lat"];
                            double longitude = (double)results[0]["lon"];

                            return (latitude, longitude);
                        }
                        else
                        {
                            Console.WriteLine($"No results found for address: {address}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Failed to get coordinates for address: {address}. Status code: {response.StatusCode}");
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP request error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while getting coordinates: {ex.Message}");
            }

            return (0, 0);
        }
    }
}
