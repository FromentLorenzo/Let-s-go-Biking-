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
        static readonly HttpClient client = new HttpClient();

        ProxyCache<JCDecauxItems> cache = new ProxyCache<JCDecauxItems>();



        public string getResponse(string url)
        {
            Task<string> response = callJCD(url);
            return response.Result;
        }

        public static async Task<string> callJCD(string request)
        {
            try
            {
                string responseBody = await client.GetStringAsync(request);
                return responseBody;
            }
            catch (Exception e)
            {

            }
            return null;
        }

        public string getAllStationsOfAContract(string chosenContract)
        {
            //récup depuis le cache
            string stations = cache.Get(chosenContract).getStations();
            Console.WriteLine(stations);
            return stations;
        }

        //Vu qu'on récupère qu'une seule station, l'appel n'est pas énorme donc on utilise pas le cache
        public string getStation(int number, string chosenContract)
        {
            Task<HttpResponseMessage> responseStations = client.GetAsync("https://api.jcdecaux.com/vls/v2/stations/"+number+"?contract="+chosenContract+ "&apiKey=e65de5172e58282b856fd72204eb35c710d1e336");
            if (responseStations.Result.IsSuccessStatusCode)
            {
                // Récupérez le contenu de la réponse
                string responseBody = responseStations.Result.ToString();
                return responseBody;
            }
            return null;
                
        }
    }
}
