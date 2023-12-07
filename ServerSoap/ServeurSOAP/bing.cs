using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ServeurSOAP
{
    public class bing
    {

        public bing()
        {
        }

        public async Task<List<string>> GetInstructions(string mode, double latDepart, double lonDepart, double latArrivee, double lonArrivee)
        {
            string apiKey = "Ak1tXbI5nLXp64HlI6LtFtYY0aMhUgDlNfjFL7RT29XiMcTgGRY0JykW4e5Z_wwQ";

            using (HttpClient client = new HttpClient())
            {

                string url = $"https://dev.virtualearth.net/REST/V1/Routes/{mode}?wp.0={latDepart},{lonDepart}&wp.1={latArrivee},{lonArrivee}&key={apiKey}";
                Console.WriteLine(url);
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    //Console.WriteLine(result); // Affichez les données d'itinéraire (en format JSON) ici

                    // Parsez les données JSON
                    JObject json = JObject.Parse(result);

                    // Obtenez les instructions si elles sont dans une structure spécifique des données JSON
                    List<string> instructions = ParseBingMapsDirections(json);

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
            string apiKey = "Ak1tXbI5nLXp64HlI6LtFtYY0aMhUgDlNfjFL7RT29XiMcTgGRY0JykW4e5Z_wwQ";

            using (HttpClient client = new HttpClient())
            {
                string url = $"https://dev.virtualearth.net/REST/V1/Routes/{mode}?wp.0={latDepart},{lonDepart}&wp.1={latArrivee},{lonArrivee}&key={apiKey}";
                Console.WriteLine(url);
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();

                    // Parse the JSON data
                    JObject json = JObject.Parse(result);

                    // Get GPS points from the response
                    List<string> gpsPoints = ParseBingMapsGPSPoints(json);

                    // Return the GPS points as a string
                    return gpsPoints;
                }
                else
                {
                    Console.WriteLine($"Erreur : {response.StatusCode}");
                }

                // In case of an error or if no GPS points are found, return an empty string
                return new List<String>();
            }
        }

        private List<string> ParseBingMapsGPSPoints(JObject json)
        {
            List<string> gpsPoints = new List<string>();

            JArray routes = (JArray)json["resourceSets"]?[0]?["resources"];
            if (routes != null && routes.HasValues)
            {
                JArray routeLegs = (JArray)routes[0]?["routeLegs"];
                if (routeLegs != null && routeLegs.HasValues)
                {
                    foreach (var routeLeg in routeLegs)
                    {
                        JArray itineraryItems = (JArray)routeLeg["itineraryItems"];
                        if (itineraryItems != null && itineraryItems.HasValues)
                        {
                            foreach (var item in itineraryItems)
                            {
                                JArray coordinates = (JArray)item?["maneuverPoint"]?["coordinates"];
                                if (coordinates != null && coordinates.HasValues)
                                {
                                    double latitude = (double)coordinates[0];
                                    double longitude = (double)coordinates[1];
                                    gpsPoints.Add($"{latitude}-{longitude}");
                                    // gpsPoints.Add($"{latitude.ToString("0.000000")},{longitude.ToString("0.000000")}");
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Aucun itinéraire trouvé dans la réponse.");
            }

            return gpsPoints;
        }





        private List<string> ParseBingMapsDirections(JObject json)
        {
            List<string> instructions = new List<string>();

            // Vérifiez si la réponse contient des itinéraires
            JArray routes = (JArray)json["resourceSets"]?[0]?["resources"];
            if (routes != null && routes.HasValues)
            {
                // Parcourez les étapes et obtenez les instructions
                JArray itineraryItems = (JArray)routes[0]?["routeLegs"]?[0]?["itineraryItems"];
                if (itineraryItems != null)
                {
                    foreach (var item in itineraryItems)
                    {
                        string instruction = item["instruction"]["text"]?.ToString();
                        //Console.WriteLine(instruction);
                        instructions.Add(instruction);
                    }
                }
            }
            else
            {
                Console.WriteLine("Aucun itinéraire trouvé dans la réponse.");
            }

            // Ajoutez d'autres cas de traitement en fonction de la structure réelle des données JSON retournées par l'API.

            return instructions;
        }
    }
}
