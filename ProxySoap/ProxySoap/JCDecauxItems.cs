using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ProxySoap
{
    public class JCDecauxItems
    {
        // Client HTTP réutilisable pour les requêtes
        public static readonly HttpClient client = new HttpClient();

        // URL de base pour les requêtes JCDecaux
        public string url = "https://api.jcdecaux.com/vls/v3/";

        // Clé API JCDecaux
        public string apiKey = "e65de5172e58282b856fd72204eb35c710d1e336";

        // Variable statique pour stocker toutes les stations d'un contrat
        public static string allStationsOfAContract;

        // Contrat sélectionné pour la requête JCDecaux
        public string contractSelected;

        // Constructeur de la classe JCDecauxItems
        public JCDecauxItems(string contractSelected)
        {
            // Initialiser l'attribut de classe
            this.contractSelected = contractSelected;

            // Appeler la méthode callJCD pour récupérer toutes les stations d'un contrat spécifique
            allStationsOfAContract = callJCD(url + "stations?contract=" + contractSelected + "&apiKey=" + apiKey).Result;
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
                // Gérer les exceptions, mais le bloc catch actuel est vide
                Console.WriteLine("Erreur lors de la requête JCDecaux : " + e.Message);
            }
            return null;
        }

        // Méthode pour obtenir toutes les stations d'un contrat
        public string getStations()
        {
            // Retourner la variable statique contenant toutes les stations du contrat
            return allStationsOfAContract;
        }
    }
}
