using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;

namespace ServeurSOAP
{
    public class OpenRouteService
    {
        public OpenRouteService() { }

        public async Task<List<string>> GetInstructions(string mode, double latDepart, double lonDepart, double latArrivee, double lonArrivee)
        {
            string apiKey = "5b3ce3597851110001cf6248ff835b523cad4528bbc75a9da6863da8";

            using (HttpClient client = new HttpClient())
            {
                string lonDepartS=FormaterDouble(lonDepart);
                string latDepartS=FormaterDouble(latDepart);
                string latArriveeS=FormaterDouble(latArrivee);
                string longArriveeS=FormaterDouble(lonArrivee);
                string url = $"https://api.openrouteservice.org/v2/directions/{mode}?api_key={apiKey}&start={lonDepartS},{latDepartS}&end={longArriveeS},{latArriveeS}";
                Console.WriteLine(url);

                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();

                    // Parsez les données JSON avec la méthode appropriée
                    List<string> instructions = ParseOpenRouteServiceDirections(result);

                    // Retournez les instructions
                    return instructions;
                }
                else
                {
                    Console.WriteLine($"Erreur : {response.StatusCode}");
                }

                // En cas d'erreur ou si aucune instruction n'est trouvée, retournez une liste vide
                return new List<string>();
            }
        }

        public async Task<List<string>> GetGPSPoints(string mode, double latDepart, double lonDepart, double latArrivee, double lonArrivee)
        {
            string apiKey = "5b3ce3597851110001cf6248ff835b523cad4528bbc75a9da6863da8";

            using (HttpClient client = new HttpClient())
            {
                string lonDepartS = FormaterDouble(lonDepart);
                string latDepartS = FormaterDouble(latDepart);
                string latArriveeS = FormaterDouble(latArrivee);
                string longArriveeS = FormaterDouble(lonArrivee);
                string url = $"https://api.openrouteservice.org/v2/directions/{mode}?api_key={apiKey}&start={lonDepartS},{latDepartS}&end={longArriveeS},{latArriveeS}";
                Console.WriteLine(url);

                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();

                    // Parsez les données JSON avec la méthode appropriée
                    List<string> instructions = ParseOpenRouteServiceGPSPoints(result);

                    // Retournez les instructions
                    return instructions;
                }
                else
                {
                    Console.WriteLine($"Erreur : {response.StatusCode}");
                }

                // En cas d'erreur ou si aucune instruction n'est trouvée, retournez une liste vide
                return new List<string>();
            }
        }

        private List<string> ParseOpenRouteServiceGPSPoints(string json)
        {
            List<string> gpsPoints = new List<string>();

            JObject response = JObject.Parse(json);

            if (response.TryGetValue("features", out var features) && features is JArray featuresArray)
            {
                foreach (var feature in featuresArray)
                {
                    ParseGeometryCoordinates(feature, gpsPoints);
                }
            }
            else
            {
                Console.WriteLine("Aucun itinéraire trouvé dans la réponse.");
            }

            return gpsPoints;
        }

        private void ParseGeometryCoordinates(JToken feature, List<string> gpsPoints)
        {
            var geometry = feature["geometry"];

            if (geometry != null && geometry is JObject geometryObject)
            {
                var coordinates = geometryObject["coordinates"];

                if (coordinates != null && coordinates is JArray coordinatesArray)
                {
                    foreach (var coordinate in coordinatesArray)
                    {
                        if (coordinate is JArray coordinateArray && coordinateArray.Count >= 2)
                        {
                            double longitude = (double)coordinateArray[0];
                            double latitude = (double)coordinateArray[1];
                            gpsPoints.Add($"{latitude}-{longitude}");
                            // gpsPoints.Add($"{latitude.ToString("0.000000")},{longitude.ToString("0.000000")}");
                        }
                    }
                }
            }
        }




        private List<string> ParseOpenRouteServiceDirections(string result)
        {
            List<string> instructions = new List<string>();

            // Parsez les données JSON d'OpenRouteService
            JObject json = JObject.Parse(result);

            // Obtenez les instructions si elles sont dans une structure spécifique des données JSON
            JArray features = (JArray)json["features"];
            if (features != null && features.HasValues)
            {
                foreach (var feature in features)
                {
                    JArray segments = (JArray)feature["properties"]["segments"];
                    if (segments != null && segments.HasValues)
                    {
                        foreach (var segment in segments)
                        {
                            JArray steps = (JArray)segment["steps"];
                            if (steps != null && steps.HasValues)
                            {
                                foreach (var step in steps)
                                {
                                    string instruction = step["instruction"]?.ToString();
                                    instructions.Add(instruction);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Aucune instruction trouvée dans la réponse d'OpenRouteService.");
            }

            return instructions;
        }

        static string FormaterDouble(double nombre)
        {
            // Utiliser la culture "en-US" pour garantir l'utilisation du point comme séparateur décimal
            CultureInfo culture = new CultureInfo("en-US");
            return nombre.ToString(culture);
        }
    }
}
