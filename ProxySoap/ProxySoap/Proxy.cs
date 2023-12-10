using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ProxySoap
{
    internal class Proxy : IProxy
    {
        // Client HTTP réutilisable pour les requêtes
        static readonly HttpClient client = new HttpClient();

        // Cache pour stocker les résultats des requêtes JCDecaux
        ProxyCache<JCDecauxItems> cache = new ProxyCache<JCDecauxItems>();

        // Méthode pour obtenir la réponse d'une URL donnée
        public string getResponse(string url)
        {
            
            Task<string> response = callJCD(url);
            return response.Result; // Peut générer une exception si la tâche n'est pas encore terminée
        }

        // Méthode asynchrone pour effectuer une requête JCDecaux et récupérer la réponse
        public static async Task<string> callJCD(string request)
        {
            try
            {
                // Effectuer une requête GET et obtenir le corps de la réponse
                string responseBody = await client.GetStringAsync(request);
                return responseBody;
            }
            catch (Exception e)
            {
                Console.WriteLine("Erreur lors de la requête JCDecaux : " + e.Message);
            }
            return null;
        }

        // Méthode pour obtenir toutes les stations d'un contrat donné (en utilisant le cache)
        public string getAllStationsOfAContract(string chosenContract)
        {
            // Récupérer depuis le cache avec une expiration de 900 secondes (15 minutes)
            try
            {
                string stations = cache.Get(chosenContract, 900).getStations();
                Console.WriteLine(stations);
                return stations;
            }
            catch (Exception e)
            {
                // Gérer les exceptions, par exemple si le contrat n'est pas dans le cache
                Console.WriteLine("Erreur lors de la récupération des stations depuis le cache : " + e.Message);
                return null;
            }
        }

        // Comme on récupère une seule station, l'appel à l'API n'est pas énorme, donc on n'utilise pas le cache
        public string getStation(int number, string chosenContract)
        {
            try
            {
                // Effectuer une requête GET pour obtenir les détails d'une station spécifique
                Task<HttpResponseMessage> responseStations = client.GetAsync("https://api.jcdecaux.com/vls/v2/stations/" + number + "?contract=" + chosenContract + "&apiKey=e65de5172e58282b856fd72204eb35c710d1e336");

                // Vérifier si la requête a réussi
                if (responseStations.Result.IsSuccessStatusCode)
                {
                    // Récupérer le contenu de la réponse
                    string responseBody = responseStations.Result.ToString();
                    return responseBody;
                }
                return null;
            }
            catch (Exception e)
            {
                // Gérer les exceptions, par exemple si la requête a échoué
                Console.WriteLine("Erreur lors de la récupération de la station : " + e.Message);
                return null;
            }
        }
    }
}
