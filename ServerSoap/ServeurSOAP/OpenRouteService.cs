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

        // Méthode pour obtenir les instructions d'itinéraire depuis OpenRouteService
        public async Task<List<string>> GetInstructions(string mode, double latDepart, double lonDepart, double latArrivee, double lonArrivee)
        {
            string apiKey = "5b3ce3597851110001cf6248ff835b523cad4528bbc75a9da6863da8";

            using (HttpClient client = new HttpClient())
            {
                // Formater les coordonnées et construire l'URL
                string lonDepartS = FormaterDouble(lonDepart);
                string latDepartS = FormaterDouble(latDepart);
                string latArriveeS = FormaterDouble(latArrivee);
                string longArriveeS = FormaterDouble(lonArrivee);
                string url = $"https://api.openrouteservice.org/v2/directions/{mode}?api_key={apiKey}&start={lonDepartS},{latDepartS}&end={longArriveeS},{latArriveeS}";
                Console.WriteLine(url);

                // Envoyer la requête HTTP
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();

                    // Parsez les données JSON pour obtenir les instructions d'itinéraire
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

        // Méthode pour obtenir les points GPS depuis OpenRouteService
        public async Task<List<string>> GetGPSPoints(string mode, double latDepart, double lonDepart, double latArrivee, double lonArrivee)
        {
            string apiKey = "5b3ce3597851110001cf6248ff835b523cad4528bbc75a9da6863da8";

            using (HttpClient client = new HttpClient())
            {
                // Formater les coordonnées et construire l'URL
                string lonDepartS = FormaterDouble(lonDepart);
                string latDepartS = FormaterDouble(latDepart);
                string latArriveeS = FormaterDouble(latArrivee);
                string longArriveeS = FormaterDouble(lonArrivee);
                string url = $"https://api.openrouteservice.org/v2/directions/{mode}?api_key={apiKey}&start={lonDepartS},{latDepartS}&end={longArriveeS},{latArriveeS}";
                Console.WriteLine(url);

                // Envoyer la requête HTTP
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();

                    // Parsez les données JSON pour obtenir les points GPS
                    List<string> gpsPoints = ParseOpenRouteServiceGPSPoints(result);

                    // Retournez les points GPS
                    return gpsPoints;
                }
                else
                {
                    Console.WriteLine($"Erreur : {response.StatusCode}");
                }

                // En cas d'erreur ou si aucun point GPS n'est trouvé, retournez une liste vide
                return new List<string>();
            }
        }

        // Méthode pour parser les points GPS depuis la réponse JSON d'OpenRouteService
        private List<string> ParseOpenRouteServiceGPSPoints(string json)
        {
            // Liste qui stockera les points GPS sous forme de chaînes
            List<string> gpsPoints = new List<string>();

            // Analyser la réponse JSON en un objet JSON
            JObject response = JObject.Parse(json);

            // Vérifier si l'objet JSON contient la clé "features" et si sa valeur est un tableau JSON
            if (response.TryGetValue("features", out var features) && features is JArray featuresArray)
            {
                // Parcourir chaque objet dans le tableau des fonctionnalités
                foreach (var feature in featuresArray)
                {
                    // Extraire les coordonnées géométriques et les ajouter à la liste des points GPS
                    ParseGeometryCoordinates(feature, gpsPoints);
                }
            }
            else
            {
                Console.WriteLine("Aucun itinéraire trouvé dans la réponse.");
            }

            // Retourner la liste des points GPS
            return gpsPoints;
        }


        // Méthode pour extraire les coordonnées géométriques depuis une fonctionnalité JSON
        private void ParseGeometryCoordinates(JToken feature, List<string> gpsPoints)
        {
            // Extraire l'objet géométrique de la fonctionnalité
            var geometry = feature["geometry"];

            // Vérifier si l'objet géométrique est présent et s'il s'agit d'un objet JSON
            if (geometry != null && geometry is JObject geometryObject)
            {
                // Extraire les coordonnées du point géométrique
                var coordinates = geometryObject["coordinates"];

                // Vérifier si les coordonnées sont présentes et si c'est un tableau JSON
                if (coordinates != null && coordinates is JArray coordinatesArray)
                {
                    // Parcourir chaque paire de coordonnées dans le tableau
                    foreach (var coordinate in coordinatesArray)
                    {
                        // Vérifier si la paire de coordonnées est un tableau JSON et a au moins deux éléments
                        if (coordinate is JArray coordinateArray && coordinateArray.Count >= 2)
                        {
                            // Extraire la latitude et la longitude et les ajouter à la liste des points GPS
                            double longitude = (double)coordinateArray[0];
                            double latitude = (double)coordinateArray[1];
                            gpsPoints.Add($"{latitude}-{longitude}");
                        }
                    }
                }
            }
        }


        // Méthode pour parser les instructions d'itinéraire depuis la réponse JSON d'OpenRouteService
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

        // Méthode pour formater un double en chaîne (utilise la culture "en-US" pour le point décimal)
        static string FormaterDouble(double nombre)
        {
            CultureInfo culture = new CultureInfo("en-US");
            return nombre.ToString(culture);
        }
    }
}
